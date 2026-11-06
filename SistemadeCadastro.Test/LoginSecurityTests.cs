using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Domain.Validators;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    // Forca bruta no login, regra de senha e revalidacao do token a cada requisicao.
    public class LoginSecurityTests
    {
        private const string Email = "user@a.com";
        private const string Password = "Senha1234";

        private static SistemaDeCadastroContext CreateContext(string dbName) =>
            new(new DbContextOptionsBuilder<SistemaDeCadastroContext>().UseInMemoryDatabase(dbName).Options,
                new TestCurrentUserService());

        private static IConfiguration JwtConfig() => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-de-teste-com-tamanho-suficiente-para-hmac-sha256",
                ["Jwt:Issuer"] = "teste",
                ["Jwt:Audience"] = "teste",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();

        private static async Task<(SistemaDeCadastroContext ctx, AuthApp app)> Setup(bool active = true)
        {
            var ctx = CreateContext("login_" + Guid.NewGuid());
            ctx.LoginAccounts.Add(new LoginAccount
            {
                Id = 10, UserId = 10, Name = "U", Email = Email, UserType = "Colaborador", InstitutionId = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password), Active = active
            });
            await ctx.SaveChangesAsync();

            var guard = new LoginAttemptGuard(new MemoryCache(new MemoryCacheOptions()));
            return (ctx, new AuthApp(new LoginAccountRepository(ctx), JwtConfig(), guard));
        }

        private static Task<ApiResponse> Login(AuthApp app, string email, string password) =>
            app.Login(new LoginRequestDTO { Email = email, Password = password });

        [Fact]
        public async Task Login_BlocksEmail_AfterFiveWrongPasswords_EvenWithRightPassword()
        {
            var (ctx, app) = await Setup();
            using var _ = ctx;

            for (var i = 0; i < LoginAttemptGuard.MaxFailedAttempts; i++)
                Assert.False((await Login(app, Email, "errada123")).Success);

            var ret = await Login(app, Email, Password);
            Assert.False(ret.Success);
            Assert.Contains("Muitas tentativas", ret.Message);
        }

        [Fact]
        public async Task Login_Success_ResetsFailureCount()
        {
            var (ctx, app) = await Setup();
            using var _ = ctx;

            for (var i = 0; i < LoginAttemptGuard.MaxFailedAttempts - 1; i++)
                await Login(app, Email, "errada123");
            Assert.True((await Login(app, Email, Password)).Success);

            for (var i = 0; i < LoginAttemptGuard.MaxFailedAttempts - 1; i++)
                await Login(app, Email, "errada123");
            Assert.True((await Login(app, Email, Password)).Success);
        }

        [Fact]
        public async Task Login_UnknownEmail_SameMessage_AndAlsoGetsBlocked()
        {
            var (ctx, app) = await Setup();
            using var _ = ctx;

            var wrongPassword = await Login(app, Email, "errada123");
            var unknownEmail = await Login(app, "ninguem@a.com", "errada123");
            Assert.Equal(wrongPassword.Message, unknownEmail.Message);

            for (var i = 1; i < LoginAttemptGuard.MaxFailedAttempts; i++)
                await Login(app, "ninguem@a.com", "errada123");
            Assert.Contains("Muitas tentativas", (await Login(app, "ninguem@a.com", "errada123")).Message);
        }

        [Fact]
        public async Task Login_InactiveAccount_IsOnlyRevealedWithRightPassword()
        {
            var (ctx, app) = await Setup(active: false);
            using var _ = ctx;

            Assert.Equal("E-mail ou senha inválidos.", (await Login(app, Email, "errada123")).Message);
            Assert.Equal("Usuário inativo.", (await Login(app, Email, Password)).Message);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("1234567", false)]
        [InlineData("12345678", false)]
        [InlineData("abcdefgh", false)]
        [InlineData("abcd1234", true)]
        public void PasswordValidator_RequiresEightCharsWithLettersAndNumbers(string password, bool valid) =>
            Assert.Equal(valid, PasswordValidator.IsValid(password));

        [Fact]
        public async Task CreateAccount_WithWeakPassword_IsRejected()
        {
            using var ctx = CreateContext("weak_" + Guid.NewGuid());
            var admin = new FakeAdmin();
            var app = new LoginAccountApp(new LoginAccountRepository(ctx), admin);

            var ret = await app.Create(new CreateLoginAccountDTO
            {
                Nome = "Fraca", Email = "fraca@a.com", Password = "123456", UserType = "Colaborador", InstitutionId = 1
            });

            Assert.False(ret.Success);
            Assert.Equal(PasswordValidator.MensagemInvalido, ret.ErrorMessage);
            Assert.False(await ctx.LoginAccounts.AnyAsync(l => l.Email == "fraca@a.com"));
        }

        private class FakeAdmin : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            public long? InstitutionId => 1;
            public long? UserId => 1;
            public string? Role => "Administrador";
        }

        // ---------------- revalidacao do token ----------------

        private static ClaimsPrincipal Token(long userId, string role, long? institutionId) =>
            new(new ClaimsIdentity(new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("institutionId", institutionId?.ToString() ?? string.Empty)
            }, "Bearer"));

        [Fact]
        public async Task Session_IsValid_WhileAccountUnchanged()
        {
            var (ctx, app) = await Setup();
            using var _ = ctx;
            Assert.True(await app.IsSessionValid(Token(10, "Colaborador", 1)));
        }

        [Fact]
        public async Task Session_IsRevoked_WhenAccountDeactivatedDeletedOrChanged()
        {
            var (ctx, app) = await Setup();
            using var _ = ctx;
            var account = await ctx.LoginAccounts.SingleAsync();

            // Papel alterado (ex.: promovido ou rebaixado) depois de emitir o token.
            Assert.False(await app.IsSessionValid(Token(10, "Administrador", 1)));
            // Instituicao diferente da do token.
            Assert.False(await app.IsSessionValid(Token(10, "Colaborador", 2)));
            Assert.False(await app.IsSessionValid(Token(10, "Colaborador", null)));

            account.Active = false;
            await ctx.SaveChangesAsync();
            Assert.False(await app.IsSessionValid(Token(10, "Colaborador", 1)));

            ctx.LoginAccounts.Remove(account);
            await ctx.SaveChangesAsync();
            Assert.False(await app.IsSessionValid(Token(10, "Colaborador", 1)));
        }
    }
}
