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

        private static async Task<(SistemaDeCadastroContext ctx, AuthApp app, CapturingEmailSender email)> Setup(bool active = true)
        {
            var ctx = CreateContext("login_" + Guid.NewGuid());
            ctx.LoginAccounts.Add(new LoginAccount
            {
                Id = 10, UserId = 10, Name = "U", Email = Email, UserType = "Colaborador", InstitutionId = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password), Active = active
            });
            await ctx.SaveChangesAsync();

            var guard = new LoginAttemptGuard(new MemoryCache(new MemoryCacheOptions()));
            var email = new CapturingEmailSender();
            return (ctx, new AuthApp(new LoginAccountRepository(ctx), JwtConfig(), guard, email), email);
        }

        // Fake de e-mail: nao envia nada, apenas guarda o ultimo corpo para os testes
        // extrairem o codigo de 6 digitos.
        private sealed class CapturingEmailSender : SistemaDeCadastro.Infra.Interface.IEmailSender
        {
            public string? LastBody { get; private set; }
            public int SendCount { get; private set; }

            public Task<bool> SendAsync(string toEmail, string subject, string htmlBody, System.Threading.CancellationToken cancellationToken = default)
            {
                LastBody = htmlBody;
                SendCount++;
                return Task.FromResult(true);
            }

            public string? ExtractCode()
            {
                if (LastBody == null) return null;
                var match = System.Text.RegularExpressions.Regex.Match(LastBody, @"\b\d{6}\b");
                return match.Success ? match.Value : null;
            }
        }

        private static Task<ApiResponse> Login(AuthApp app, string email, string password) =>
            app.Login(new LoginRequestDTO { Email = email, Password = password });

        [Fact]
        public async Task Login_BlocksEmail_AfterFiveWrongPasswords_EvenWithRightPassword()
        {
            var (ctx, app, _) = await Setup();
            using var _ctx = ctx;

            for (var i = 0; i < LoginAttemptGuard.MaxFailedAttempts; i++)
                Assert.False((await Login(app, Email, "errada123")).Success);

            var ret = await Login(app, Email, Password);
            Assert.False(ret.Success);
            Assert.Contains("Muitas tentativas", ret.Message);
        }

        [Fact]
        public async Task Login_Success_ResetsFailureCount()
        {
            var (ctx, app, _) = await Setup();
            using var _ctx = ctx;

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
            var (ctx, app, _) = await Setup();
            using var _ctx = ctx;

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
            var (ctx, app, _) = await Setup(active: false);
            using var _ctx = ctx;

            Assert.Equal("E-mail ou senha inválidos.", (await Login(app, Email, "errada123")).Message);
            Assert.Equal("Usuário inativo.", (await Login(app, Email, Password)).Message);
        }

        // ---------------- esqueci minha senha ----------------

        [Fact]
        public async Task ForgotPassword_ThenReset_WithCorrectCode_AllowsLoginWithNewPassword()
        {
            var (ctx, app, email) = await Setup();
            using var _ctx = ctx;

            var forgot = await app.RequestPasswordReset(new ForgotPasswordRequestDTO { Email = Email });
            Assert.True(forgot.Success);
            Assert.Equal(1, email.SendCount);

            var code = email.ExtractCode();
            Assert.NotNull(code);

            const string NewPassword = "NovaSenha123";
            var reset = await app.ResetPassword(new ResetPasswordRequestDTO
            {
                Email = Email, Code = code!, NewPassword = NewPassword
            });
            Assert.True(reset.Success);

            // Senha antiga nao vale mais; a nova funciona.
            Assert.False((await Login(app, Email, Password)).Success);
            Assert.True((await Login(app, Email, NewPassword)).Success);
        }

        [Fact]
        public async Task ForgotPassword_UnknownEmail_SameNeutralMessage_AndSendsNothing()
        {
            var (ctx, app, email) = await Setup();
            using var _ctx = ctx;

            var known = await app.RequestPasswordReset(new ForgotPasswordRequestDTO { Email = Email });
            var unknown = await app.RequestPasswordReset(new ForgotPasswordRequestDTO { Email = "ninguem@a.com" });

            Assert.Equal(known.Message, unknown.Message);
            // So houve envio para a conta que existe.
            Assert.Equal(1, email.SendCount);
        }

        [Fact]
        public async Task ResetPassword_WithWrongCode_IsRejected()
        {
            var (ctx, app, _) = await Setup();
            using var _ctx = ctx;

            await app.RequestPasswordReset(new ForgotPasswordRequestDTO { Email = Email });

            var reset = await app.ResetPassword(new ResetPasswordRequestDTO
            {
                Email = Email, Code = "000000", NewPassword = "NovaSenha123"
            });

            Assert.False(reset.Success);
            // A senha original continua valendo.
            Assert.True((await Login(app, Email, Password)).Success);
        }

        [Fact]
        public async Task ResetPassword_CodeIsSingleUse()
        {
            var (ctx, app, email) = await Setup();
            using var _ctx = ctx;

            await app.RequestPasswordReset(new ForgotPasswordRequestDTO { Email = Email });
            var code = email.ExtractCode()!;

            Assert.True((await app.ResetPassword(new ResetPasswordRequestDTO
            {
                Email = Email, Code = code, NewPassword = "NovaSenha123"
            })).Success);

            // Mesmo codigo nao pode ser reutilizado.
            Assert.False((await app.ResetPassword(new ResetPasswordRequestDTO
            {
                Email = Email, Code = code, NewPassword = "OutraSenha123"
            })).Success);
        }

        [Fact]
        public async Task ResetPassword_WithWeakPassword_IsRejected()
        {
            var (ctx, app, email) = await Setup();
            using var _ctx = ctx;

            await app.RequestPasswordReset(new ForgotPasswordRequestDTO { Email = Email });
            var code = email.ExtractCode()!;

            var reset = await app.ResetPassword(new ResetPasswordRequestDTO
            {
                Email = Email, Code = code, NewPassword = "123456"
            });

            Assert.False(reset.Success);
            Assert.Equal(PasswordValidator.MensagemInvalido, reset.Message);
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
            var (ctx, app, _) = await Setup();
            using var _ctx = ctx;
            Assert.True(await app.IsSessionValid(Token(10, "Colaborador", 1)));
        }

        [Fact]
        public async Task Session_IsRevoked_WhenAccountDeactivatedDeletedOrChanged()
        {
            var (ctx, app, _) = await Setup();
            using var _ctx = ctx;
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
