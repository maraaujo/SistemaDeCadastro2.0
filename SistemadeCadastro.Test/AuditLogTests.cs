using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using Xunit;

namespace SistemadeCadastro.Test
{
    public class AuditLogTests
    {
        private class FakeUser : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            private readonly long? _userId;
            public FakeUser(long? userId) => _userId = userId;
            public long? InstitutionId => null;
            public long? UserId => _userId;
            public string? IpAddress => "10.0.0.1";
        }

        private static SistemaDeCadastroContext CreateContext(string dbName, long? userId = 42)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new SistemaDeCadastroContext(options, new FakeUser(userId));
        }

        // Na vida real o usuario logado sempre existe (log_acesso.id_usuario tem FK
        // para contas_login). Cadastra a conta dele sem auditoria (sem usuario).
        private static async Task<SistemaDeCadastroContext> CreateContextWithLoggedUser(string dbName)
        {
            using (var seed = CreateContext(dbName, userId: null))
            {
                seed.LoginAccounts.Add(new LoginAccount
                {
                    UserId = 42,
                    Name = "Usuário logado",
                    Email = "logado@teste.com",
                    PasswordHash = "x",
                    UserType = "Administrador",
                });
                await seed.SaveChangesAsync();
            }

            return CreateContext(dbName);
        }

        private static Medicine NewMedicine() => new()
        {
            Name = "Dipirona",
            Dosage = "500mg",
            Description = "Analgésico",
            AdministrationRoute = "Oral",
        };

        [Fact]
        public async Task Create_Update_Delete_Generate_Audit_Logs()
        {
            using var context = await CreateContextWithLoggedUser(nameof(Create_Update_Delete_Generate_Audit_Logs));

            var medicine = NewMedicine();
            context.Medicines.Add(medicine);
            await context.SaveChangesAsync();

            medicine.Dosage = "1g";
            await context.SaveChangesAsync();

            context.Medicines.Remove(medicine);
            await context.SaveChangesAsync();

            var logs = await context.AccessLogs.OrderBy(l => l.Id).ToListAsync();

            Assert.Equal(3, logs.Count);
            Assert.All(logs, l =>
            {
                Assert.Equal(42, l.UserId);
                Assert.Equal("10.0.0.1", l.IpAddress);
            });

            Assert.Equal($"Criou Medicamento #{medicine.Id}", logs[0].Action);
            Assert.Equal("Name: Dipirona", logs[0].Observation);

            Assert.Equal($"Alterou Medicamento #{medicine.Id}", logs[1].Action);
            Assert.Equal("Dosage: 500mg → 1g", logs[1].Observation);

            Assert.Equal($"Excluiu Medicamento #{medicine.Id}", logs[2].Action);
        }

        [Fact]
        public async Task Password_Value_Is_Never_Logged()
        {
            using var context = await CreateContextWithLoggedUser(nameof(Password_Value_Is_Never_Logged));

            var account = new LoginAccount
            {
                UserId = 7,
                Name = "Ana",
                Email = "ana@teste.com",
                PasswordHash = "hash-antigo",
                UserType = "Administrador",
            };
            context.LoginAccounts.Add(account);
            await context.SaveChangesAsync();

            account.PasswordHash = "hash-novo";
            await context.SaveChangesAsync();

            var update = await context.AccessLogs.OrderBy(l => l.Id).LastAsync();

            Assert.StartsWith("Alterou Conta de acesso", update.Action);
            Assert.Equal("PasswordHash: (alterado)", update.Observation);
            Assert.DoesNotContain("hash-", update.Observation);
        }

        [Fact]
        public async Task Without_Logged_User_No_Log_Is_Written()
        {
            using var context = CreateContext(nameof(Without_Logged_User_No_Log_Is_Written), userId: null);

            context.Medicines.Add(NewMedicine());
            await context.SaveChangesAsync();

            Assert.Empty(await context.AccessLogs.ToListAsync());
        }
    }
}
