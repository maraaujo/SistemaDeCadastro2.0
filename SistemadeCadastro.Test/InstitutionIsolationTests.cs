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
    // Garante que um usuario da instituicao A nao le, edita nem apaga dados da instituicao B.
    public class InstitutionIsolationTests
    {
        private const long InstA = 1;
        private const long InstB = 2;

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

        private static SistemaDeCadastroContext CreateContext(string dbName, FakeUser user)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new SistemaDeCadastroContext(options, user);
        }

        // Admin da plataforma sem usuario logado: enxerga tudo e nao grava auditoria.
        private static SistemaDeCadastroContext Unfiltered(string dbName) =>
            CreateContext(dbName, new FakeUser(null, null, "Administrador"));

        private static string NewDb() => "isolation_" + Guid.NewGuid();

        private class Seed
        {
            public Patient PatientA = null!, PatientB = null!;
            public Responsible ResponsibleB = null!;
            public PatientClinicalCondition ConditionB = null!;
            public MedicinePatientClinicalCondition MedicineB = null!;
            public Employee EmployeeB = null!;
            public PatientEmployee PatientEmployeeB = null!;
            public Appointment AppointmentB = null!;
            public Payment PaymentB = null!;
        }

        private static Patient NewPatient(string name, long institutionId) => new()
        {
            Name = name, Gender = "F", Cpf = name, Observations = "", BirthDate = DateTime.Today,
            CreatedAt = DateTime.Now, InstitutionId = institutionId
        };

        private static Responsible NewResponsible(long patientId) => new()
        {
            PatientId = patientId, Name = "Resp", Phone = "11999999999", Relationship = "Mae", Address = "Rua"
        };

        private static async Task<Seed> SeedTwoInstitutions(string dbName)
        {
            using var ctx = Unfiltered(dbName);
            var s = new Seed
            {
                PatientA = NewPatient("A", InstA),
                PatientB = NewPatient("B", InstB),
                EmployeeB = new Employee
                {
                    Name = "EB", Cpf = "1", Position = "P", Phone = "0", Email = "eb@b.com",
                    AdmissionDate = DateTime.Now, InstitutionId = InstB
                }
            };
            ctx.Patients.AddRange(s.PatientA, s.PatientB);
            ctx.Employees.Add(s.EmployeeB);
            await ctx.SaveChangesAsync();

            ctx.Responsibles.Add(NewResponsible(s.PatientA.Id));
            s.ResponsibleB = NewResponsible(s.PatientB.Id);
            ctx.Responsibles.Add(s.ResponsibleB);

            s.ConditionB = new PatientClinicalCondition { PatientId = s.PatientB.Id, ClinicalConditionId = 1, Observations = "B" };
            ctx.PatientClinicalConditions.Add(s.ConditionB);

            s.PatientEmployeeB = new PatientEmployee { PatientId = s.PatientB.Id, EmployeeId = s.EmployeeB.Id, ResponsibilityFunction = "F" };
            ctx.PatientEmployees.Add(s.PatientEmployeeB);

            s.AppointmentB = new Appointment
            {
                PatientId = s.PatientB.Id, UserId = 99, InstitutionId = InstB, AppointmentType = "T",
                DateTime = DateTime.Now, Responsible = "R", Status = "S", Observations = "O"
            };
            ctx.Appointments.Add(s.AppointmentB);
            await ctx.SaveChangesAsync();

            s.MedicineB = new MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = s.ConditionB.Id, MedicineId = 1, PrescribedDosage = "1", Frequency = "1x", Observations = ""
            };
            ctx.MedicinePatientClinicalConditions.Add(s.MedicineB);

            s.PaymentB = new Payment
            {
                AppointmentId = s.AppointmentB.Id, UserId = 99, TotalAmount = 10, PaymentMethod = "Pix",
                PaymentDate = DateTime.Now, Status = "Pago", ExternalTransactionId = Guid.NewGuid().ToString()
            };
            ctx.Payments.Add(s.PaymentB);
            await ctx.SaveChangesAsync();

            return s;
        }

        // ------------------------------------------------------------------
        // Item 4: entidades filhas filtradas pela instituicao do acolhido/agendamento
        // ------------------------------------------------------------------

        [Fact]
        public async Task ChildEntities_FromOtherInstitution_AreNotVisible()
        {
            var db = NewDb();
            var s = await SeedTwoInstitutions(db);

            using var ctx = CreateContext(db, new FakeUser(InstA));

            var responsibles = await new ResponsibleRepository(ctx).GetAll();
            Assert.Single(responsibles);
            Assert.All(responsibles, r => Assert.Equal(s.PatientA.Id, r.PatientId));

            Assert.Empty(await new PatientClinicalConditionRepository(ctx).FindBy(x => x.Id == s.ConditionB.Id));
            Assert.Empty(await new MedicinePatientClinicalConditionRepository(ctx).FindBy(x => x.Id == s.MedicineB.Id));
            Assert.Empty(await new PatientEmployeeRepository(ctx).FindBy(x => x.Id == s.PatientEmployeeB.Id));
            Assert.Empty(await new PaymentRepository(ctx).GetAll());

            // A propria instituicao B continua enxergando os seus registros.
            using var ctxB = CreateContext(db, new FakeUser(InstB));
            Assert.NotEmpty(await new PatientClinicalConditionRepository(ctxB).FindBy(x => x.Id == s.ConditionB.Id));
            Assert.NotEmpty(await new MedicinePatientClinicalConditionRepository(ctxB).FindBy(x => x.Id == s.MedicineB.Id));
            Assert.NotEmpty(await new PaymentRepository(ctxB).GetAll());
        }

        [Fact]
        public async Task ResponsibleApp_CannotReadUpdateOrDelete_OtherInstitution()
        {
            var db = NewDb();
            var s = await SeedTwoInstitutions(db);

            using (var ctx = CreateContext(db, new FakeUser(InstA)))
            {
                var app = new ResponsibleApp(new ResponsibleRepository(ctx), new PatientRepository(ctx));

                Assert.Null(await app.GetById(s.ResponsibleB.Id));

                var update = await app.Update(new UpdateResponsibleDTO
                {
                    Id = s.ResponsibleB.Id, PatientId = s.PatientB.Id, Name = "Hacked", Phone = "", Relationship = "", Address = ""
                });
                Assert.False(update.Success);

                await app.Delete(s.ResponsibleB.Id);

                // Item 5: nao cria responsavel apontando para acolhido de outra instituicao.
                var create = await app.Create(new CreateResponsibleDTO
                {
                    PatientId = s.PatientB.Id, Name = "Intruso", Phone = "", Relationship = "", Address = ""
                });
                Assert.False(create.Success);
            }

            using var check = Unfiltered(db);
            var responsibleB = await check.Responsibles.SingleAsync(r => r.Id == s.ResponsibleB.Id);
            Assert.Equal("Resp", responsibleB.Name);
            Assert.Equal(1, await check.Responsibles.CountAsync(r => r.PatientId == s.PatientB.Id));
        }

        [Fact]
        public async Task PatientClinicalConditionApp_Update_DoesNotOverwriteOtherInstitution()
        {
            var db = NewDb();
            var s = await SeedTwoInstitutions(db);

            using (var ctx = CreateContext(db, new FakeUser(InstA)))
            {
                var app = new PatientClinicalConditionApp(
                    new PatientClinicalConditionRepository(ctx),
                    new PatientRepository(ctx),
                    new MedicinePatientClinicalConditionRepository(ctx),
                    new MedicationAdministrationRepository(ctx));

                var ret = await app.Update(new PatientClinicalCondition
                {
                    Id = s.ConditionB.Id, PatientId = s.PatientA.Id, ClinicalConditionId = 1, Observations = "Hacked"
                });
                Assert.False(ret.Success);

                await app.Delete(s.ConditionB.Id);
            }

            using var check = Unfiltered(db);
            var condition = await check.PatientClinicalConditions.SingleAsync(c => c.Id == s.ConditionB.Id);
            Assert.Equal("B", condition.Observations);
            Assert.Equal(s.PatientB.Id, condition.PatientId);
            Assert.True(await check.MedicinePatientClinicalConditions.AnyAsync(m => m.Id == s.MedicineB.Id));
        }

        [Fact]
        public async Task PatientEmployeeApp_CannotLinkPatientOfOtherInstitution()
        {
            var db = NewDb();
            var s = await SeedTwoInstitutions(db);

            using (var ctx = CreateContext(db, new FakeUser(InstA)))
            {
                var app = new PatientEmployeeApp(new PatientEmployeeRepository(ctx), new PatientRepository(ctx), new EmployeeRepository(ctx));

                Assert.Null(await app.GetById(s.PatientEmployeeB.Id));

                var create = await app.Create(new CreatePatientEmployeeDTO
                {
                    PatientId = s.PatientB.Id, EmployeeId = s.EmployeeB.Id, ResponsibilityFunction = "F"
                });
                Assert.False(create.Success);

                var update = await app.Update(new UpdatePatientEmployeeDTO
                {
                    Id = s.PatientEmployeeB.Id, PatientId = s.PatientA.Id, EmployeeId = s.EmployeeB.Id, ResponsibilityFunction = "Hacked"
                });
                Assert.False(update.Success);
            }

            using var check = Unfiltered(db);
            Assert.Equal(1, await check.PatientEmployees.CountAsync());
            Assert.Equal("F", (await check.PatientEmployees.SingleAsync()).ResponsibilityFunction);
        }

        // ------------------------------------------------------------------
        // Item 2: DeletePatient nao apaga dependentes de acolhido de outra instituicao
        // ------------------------------------------------------------------

        [Fact]
        public async Task DeletePatient_OfOtherInstitution_DeletesNothing()
        {
            var db = NewDb();
            var s = await SeedTwoInstitutions(db);

            using (var ctx = CreateContext(db, new FakeUser(InstA)))
            {
                var app = new PatientApp(
                    new PatientRepository(ctx),
                    new ResponsibleRepository(ctx),
                    new PatientEmployeeRepository(ctx),
                    new PatientClinicalConditionRepository(ctx),
                    new AppointmentRepository(ctx),
                    new PaymentRepository(ctx),
                    new MedicinePatientClinicalConditionRepository(ctx),
                    new MedicineRepository(ctx),
                    new FakeUser(InstA));

                var ret = await app.DeletePatient(s.PatientB.Id);
                Assert.False(ret.Success);
            }

            using var check = Unfiltered(db);
            Assert.True(await check.Patients.AnyAsync(p => p.Id == s.PatientB.Id));
            Assert.True(await check.Responsibles.AnyAsync(r => r.PatientId == s.PatientB.Id));
            Assert.True(await check.PatientClinicalConditions.AnyAsync(c => c.PatientId == s.PatientB.Id));
            Assert.True(await check.Appointments.AnyAsync(a => a.PatientId == s.PatientB.Id));
            Assert.True(await check.Payments.AnyAsync(p => p.Id == s.PaymentB.Id));
        }

        // ------------------------------------------------------------------
        // Token sem instituicao: so o admin da plataforma ve tudo (fail-closed)
        // ------------------------------------------------------------------

        [Fact]
        public async Task TokenWithoutInstitution_NotPlatformAdmin_SeesNothingAndCannotCreate()
        {
            var db = NewDb();
            var s = await SeedTwoInstitutions(db);

            var orphan = new FakeUser(null, 999, "Colaborador");
            using (var ctx = CreateContext(db, orphan))
            {
                Assert.Empty(await new PatientRepository(ctx).GetAll());
                Assert.Empty(await new ResponsibleRepository(ctx).GetAll());
                Assert.Empty(await new EmployeeRepository(ctx).GetAll());
                Assert.Empty(await new AppointmentRepository(ctx).GetAll());
                Assert.Empty(await new PaymentRepository(ctx).GetAll());
                Assert.Empty(await new MedicinePatientClinicalConditionRepository(ctx).GetAll());

                var responsibleApp = new ResponsibleApp(new ResponsibleRepository(ctx), new PatientRepository(ctx));
                var create = await responsibleApp.Create(new CreateResponsibleDTO
                {
                    PatientId = s.PatientB.Id, Name = "Intruso", Phone = "", Relationship = "", Address = ""
                });
                Assert.False(create.Success);

                var patientApp = new PatientApp(
                    new PatientRepository(ctx), new ResponsibleRepository(ctx), new PatientEmployeeRepository(ctx),
                    new PatientClinicalConditionRepository(ctx), new AppointmentRepository(ctx), new PaymentRepository(ctx),
                    new MedicinePatientClinicalConditionRepository(ctx), new MedicineRepository(ctx), orphan);
                var createPatient = await patientApp.CreatePatient(new CreatePatientDTO { Name = "Intruso", Gender = "F" });
                Assert.False(createPatient.Success);
                Assert.False((await patientApp.DeletePatient(s.PatientB.Id)).Success);
            }

            using var check = Unfiltered(db);
            Assert.Equal(2, await check.Patients.CountAsync());
            Assert.Equal(2, await check.Responsibles.CountAsync());
        }

        [Fact]
        public async Task PlatformAdmin_SeesAllInstitutions()
        {
            var db = NewDb();
            await SeedTwoInstitutions(db);

            using var ctx = CreateContext(db, new FakeUser(null, 999, "Administrador"));
            Assert.Equal(2, (await new PatientRepository(ctx).GetAll()).Count);
            Assert.Equal(2, (await new ResponsibleRepository(ctx).GetAll()).Count);
            Assert.Single(await new PaymentRepository(ctx).GetAll());
        }

        // ------------------------------------------------------------------
        // Item 1: contas de acesso
        // ------------------------------------------------------------------

        private class Accounts
        {
            public LoginAccount AdminA = null!, UserA = null!, AdminB = null!, PlatformAdmin = null!;
        }

        private static LoginAccount NewAccount(long id, string email, string role, long? institutionId) => new()
        {
            Id = id, UserId = id, Name = email, Email = email, PasswordHash = "x", UserType = role, InstitutionId = institutionId
        };

        private static async Task<Accounts> SeedAccounts(string dbName)
        {
            using var ctx = Unfiltered(dbName);
            var a = new Accounts
            {
                AdminA = NewAccount(1, "admin@a.com", "Administrador", InstA),
                UserA = NewAccount(2, "user@a.com", "Colaborador", InstA),
                AdminB = NewAccount(3, "admin@b.com", "Administrador", InstB),
                PlatformAdmin = NewAccount(4, "root@saas.com", "Administrador", null)
            };
            ctx.LoginAccounts.AddRange(a.AdminA, a.UserA, a.AdminB, a.PlatformAdmin);
            await ctx.SaveChangesAsync();
            return a;
        }

        private static LoginAccountApp AccountApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new LoginAccountRepository(ctx), user);

        private static FakeUser As(LoginAccount account) => new(account.InstitutionId, account.UserId, account.UserType);

        [Fact]
        public async Task LoginAccount_InstitutionAdmin_SeesOnlyOwnInstitution()
        {
            var db = NewDb();
            var a = await SeedAccounts(db);

            using var ctx = CreateContext(db, As(a.AdminA));
            var app = AccountApp(ctx, As(a.AdminA));

            var all = await app.GetAll();
            Assert.Equal(new long[] { a.AdminA.Id, a.UserA.Id }, all.Select(x => x.Id).OrderBy(x => x).ToArray());

            Assert.Null(await app.GetById(a.AdminB.Id));
            Assert.Null(await app.GetById(a.PlatformAdmin.Id));
            Assert.NotNull(await app.GetById(a.UserA.Id));
        }

        [Fact]
        public async Task LoginAccount_InstitutionAdmin_CannotUpdateOrDeleteOtherInstitution()
        {
            var db = NewDb();
            var a = await SeedAccounts(db);

            using (var ctx = CreateContext(db, As(a.AdminA)))
            {
                var app = AccountApp(ctx, As(a.AdminA));

                var update = await app.Update(new UpdateLoginAccountDTO
                {
                    Id = a.AdminB.Id, Name = "Hacked", Email = "hacked@b.com", UserType = "Colaborador", Active = false
                });
                Assert.False(update.Success);

                var delete = await app.Delete(a.AdminB.Id);
                Assert.False(delete.Success);
            }

            using var check = Unfiltered(db);
            var adminB = await check.LoginAccounts.SingleAsync(l => l.Id == a.AdminB.Id);
            Assert.Equal("admin@b.com", adminB.Email);
            Assert.Equal("Administrador", adminB.UserType);
            Assert.True(adminB.Active);
        }

        [Fact]
        public async Task LoginAccount_InstitutionAdmin_CannotCreatePlatformAdminOrAccountInOtherInstitution()
        {
            var db = NewDb();
            var a = await SeedAccounts(db);

            // Um contexto por chamada. Obs.: o Create grava a conta e depois tenta copiar
            // Id -> UserId (chave alternativa), o que o EF recusa; por isso o teste confere
            // a linha gravada em vez de ret.Success (bug pre-existente, fora deste escopo).
            using (var ctx = CreateContext(db, As(a.AdminA)))
            {
                await AccountApp(ctx, As(a.AdminA)).Create(new CreateLoginAccountDTO
                {
                    Nome = "x", Email = "novo-admin@a.com", Password = "123456", UserType = "Administrador", InstitutionId = null
                });
            }
            using (var ctx = CreateContext(db, As(a.AdminA)))
            {
                await AccountApp(ctx, As(a.AdminA)).Create(new CreateLoginAccountDTO
                {
                    Nome = "y", Email = "intruso@a.com", Password = "123456", UserType = "Colaborador", InstitutionId = InstB
                });
            }

            using var check = Unfiltered(db);
            Assert.Equal(InstA, (await check.LoginAccounts.SingleAsync(l => l.Email == "novo-admin@a.com")).InstitutionId);
            Assert.Equal(InstA, (await check.LoginAccounts.SingleAsync(l => l.Email == "intruso@a.com")).InstitutionId);
        }

        [Fact]
        public async Task LoginAccount_RegularUser_CannotCreateOrManage_AndCannotChangeOwnRole()
        {
            var db = NewDb();
            var a = await SeedAccounts(db);

            using (var ctx = CreateContext(db, As(a.UserA)))
            {
                var app = AccountApp(ctx, As(a.UserA));

                var create = await app.Create(new CreateLoginAccountDTO
                {
                    Nome = "x", Email = "root2@saas.com", Password = "123456", UserType = "Administrador", InstitutionId = null
                });
                Assert.False(create.Success);

                // So enxerga a propria conta.
                Assert.Equal(new[] { a.UserA.Id }, (await app.GetAll()).Select(x => x.Id).ToArray());
                Assert.Null(await app.GetById(a.AdminA.Id));
                Assert.False((await app.Delete(a.AdminA.Id)).Success);

                // Edita o proprio perfil, mas nao promove o proprio papel.
                var update = await app.Update(new UpdateLoginAccountDTO
                {
                    Id = a.UserA.Id, Name = "Novo nome", Email = "user@a.com", UserType = "Administrador", Active = true
                });
                Assert.True(update.Success, update.ErrorMessage);
            }

            using var check = Unfiltered(db);
            Assert.False(await check.LoginAccounts.AnyAsync(l => l.Email == "root2@saas.com"));
            var userA = await check.LoginAccounts.SingleAsync(l => l.Id == a.UserA.Id);
            Assert.Equal("Novo nome", userA.Name);
            Assert.Equal("Colaborador", userA.UserType);
        }

        [Fact]
        public async Task LoginAccount_PlatformAdmin_SeesAllInstitutions()
        {
            var db = NewDb();
            var a = await SeedAccounts(db);

            using var ctx = CreateContext(db, As(a.PlatformAdmin));
            var app = AccountApp(ctx, As(a.PlatformAdmin));

            Assert.Equal(4, (await app.GetAll()).Count);
            Assert.NotNull(await app.GetById(a.AdminB.Id));
        }

        [Fact]
        public async Task LoginAccount_UserWithoutInstitutionButNotAdmin_IsNotPlatformAdmin()
        {
            var db = NewDb();
            var a = await SeedAccounts(db);

            // Token sem instituicao mas com papel comum nao ganha visao global.
            var orphan = new FakeUser(null, 999, "Colaborador");
            using var ctx = CreateContext(db, orphan);
            var app = AccountApp(ctx, orphan);

            Assert.Empty(await app.GetAll());
            Assert.Null(await app.GetById(a.AdminB.Id));
            Assert.False((await app.Delete(a.AdminB.Id)).Success);
        }
    }
}
