using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    // Cadastro de conta (Nome gravado, Id = UserId, sem erro na resposta) e exclusao de
    // administracao de medicamento (antes entrava em recursao e derrubava a API).
    public class LoginAccountCreateAndDeleteTests
    {
        private const long Inst = 1;

        private class FakeUser : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            public FakeUser(long? institutionId, long? userId = null, string? role = null)
            {
                InstitutionId = institutionId;
                UserId = userId;
                Role = role;
            }
            public long? InstitutionId { get; }
            public long? UserId { get; }
            public string? Role { get; }
        }

        private static readonly FakeUser PlatformAdmin = new(null, 1, "Administrador");

        private static SistemaDeCadastroContext CreateContext(string dbName, FakeUser user)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new SistemaDeCadastroContext(options, user);
        }

        private static string NewDb() => "account_" + Guid.NewGuid();

        // Conta do admin que faz os cadastros (id 1), gravada sem auditoria.
        private static async Task SeedPlatformAdmin(string db)
        {
            using var ctx = CreateContext(db, new FakeUser(null, null, "Administrador"));
            ctx.LoginAccounts.Add(new LoginAccount
            {
                Id = 1, UserId = 1, Name = "Root", Email = "root@saas.com", PasswordHash = "x", UserType = "Administrador"
            });
            await ctx.SaveChangesAsync();
        }

        private static CreateLoginAccountDTO NewAccount(string nome, string email) => new()
        {
            Nome = nome, Email = email, Password = "12345678", UserType = "Colaborador", InstitutionId = Inst
        };

        [Fact]
        public async Task Create_SavesName_AndUserIdEqualsId()
        {
            var db = NewDb();
            await SeedPlatformAdmin(db);

            using (var ctx = CreateContext(db, PlatformAdmin))
            {
                var app = new LoginAccountApp(new LoginAccountRepository(ctx), PlatformAdmin);

                var first = await app.Create(NewAccount("  Maria Souza ", "maria@a.com"));
                Assert.True(first.Success, first.ErrorMessage);

                var second = await app.Create(NewAccount("João Lima", "joao@a.com"));
                Assert.True(second.Success, second.ErrorMessage);
            }

            using var check = CreateContext(db, PlatformAdmin);
            var maria = await check.LoginAccounts.SingleAsync(l => l.Email == "maria@a.com");
            var joao = await check.LoginAccounts.SingleAsync(l => l.Email == "joao@a.com");

            Assert.Equal("Maria Souza", maria.Name);
            Assert.Equal("João Lima", joao.Name);
            Assert.Equal(maria.Id, maria.UserId);
            Assert.Equal(joao.Id, joao.UserId);
            Assert.NotEqual(0, maria.UserId);
            Assert.NotEqual(maria.UserId, joao.UserId);
        }

        [Fact]
        public async Task Create_WithoutName_ReturnsClearMessage()
        {
            var db = NewDb();
            await SeedPlatformAdmin(db);

            using var ctx = CreateContext(db, PlatformAdmin);
            var app = new LoginAccountApp(new LoginAccountRepository(ctx), PlatformAdmin);

            var ret = await app.Create(NewAccount("   ", "sem-nome@a.com"));

            Assert.False(ret.Success);
            Assert.Equal("Nome deve ser preenchido.", ret.ErrorMessage);
            Assert.False(await ctx.LoginAccounts.AnyAsync(l => l.Email == "sem-nome@a.com"));
        }

        private static async Task<long> SeedAdministration(string db, long institutionId)
        {
            using var ctx = CreateContext(db, new FakeUser(null, null, "Administrador"));
            var patient = new Patient
            {
                Name = "P" + institutionId, Gender = "F", Cpf = "c" + institutionId, Observations = "",
                BirthDate = DateTime.Today, CreatedAt = DateTime.Now, InstitutionId = institutionId
            };
            var employee = new Employee
            {
                Name = "E" + institutionId, Cpf = "e" + institutionId, Position = "P", Phone = "0",
                Email = $"e{institutionId}@e.com", AdmissionDate = DateTime.Now, InstitutionId = institutionId
            };
            ctx.Patients.Add(patient);
            ctx.Employees.Add(employee);
            await ctx.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = 1, Observations = "" };
            ctx.PatientClinicalConditions.Add(pcc);
            await ctx.SaveChangesAsync();

            var mpcc = new MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = pcc.Id, MedicineId = 1, PrescribedDosage = "1", Frequency = "1x", Observations = ""
            };
            ctx.MedicinePatientClinicalConditions.Add(mpcc);
            await ctx.SaveChangesAsync();

            var administration = new MedicationAdministration
            {
                PatientId = patient.Id, EmployeeId = employee.Id, MedicinePatientClinicalConditionId = mpcc.Id,
                ScheduledDateTime = DateTime.Now, Status = "Administered", Observations = "", CreatedAt = DateTime.Now
            };
            ctx.MedicationAdministrations.Add(administration);
            await ctx.SaveChangesAsync();

            return administration.Id;
        }

        private static MedicationAdministrationApp AdministrationApp(SistemaDeCadastroContext ctx) =>
            new(new MedicationAdministrationRepository(ctx),
                new MedicinePatientClinicalConditionRepository(ctx),
                new PatientRepository(ctx),
                new EmployeeRepository(ctx));

        [Fact]
        public async Task DeleteAdministration_RemovesRecord_WithoutRecursion()
        {
            var db = NewDb();
            var id = await SeedAdministration(db, Inst);
            var user = new FakeUser(Inst, 10, "Colaborador");

            using (var ctx = CreateContext(db, user))
            {
                var ret = await AdministrationApp(ctx).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, new FakeUser(null, null, "Administrador"));
            Assert.False(await check.MedicationAdministrations.AnyAsync(m => m.Id == id));
        }

        [Fact]
        public async Task DeleteAdministration_NotFoundOrOtherInstitution_ReturnsError()
        {
            var db = NewDb();
            var otherInstitutionId = await SeedAdministration(db, 2);
            var user = new FakeUser(Inst, 10, "Colaborador");

            using (var ctx = CreateContext(db, user))
            {
                var app = AdministrationApp(ctx);

                var missing = await app.Delete(999);
                Assert.False(missing.Success);
                Assert.Equal("Administração de medicamento não encontrada.", missing.ErrorMessage);

                var other = await app.Delete(otherInstitutionId);
                Assert.False(other.Success);
            }

            using var check = CreateContext(db, new FakeUser(null, null, "Administrador"));
            Assert.True(await check.MedicationAdministrations.AnyAsync(m => m.Id == otherInstitutionId));
        }
    }
}
