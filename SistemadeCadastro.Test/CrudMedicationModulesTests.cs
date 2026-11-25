using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    // Testes de CRUD de ponta a ponta (App real + repositorios reais + contexto InMemory)
    // para: funcionario, vinculo acolhido-funcionario, medicamento, medicamento programado,
    // administracao de medicamento e agendamento. Cada teste usa um banco novo; a acao roda
    // num contexto e a conferencia em outro (contexto novo), para provar que foi gravado.
    // Todos os controllers desses modulos usam apenas [Authorize] (qualquer usuario logado),
    // entao o usuario de teste e um colaborador comum de uma instituicao, como na API.
    public class CrudMedicationModulesTests
    {
        private const long InstA = 1;
        private const long InstB = 2;
        private const long UserA = 10;
        private const long UserB = 20;

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

        // Usuario da instituicao 1 (como viria do JWT) e usuario da instituicao 2.
        private static readonly FakeUser UsuarioA = new(InstA, UserA, "Colaborador");
        private static readonly FakeUser UsuarioB = new(InstB, UserB, "Colaborador");

        // Admin da plataforma sem UserId: usado so para semear e conferir (enxerga tudo, sem auditoria).
        private static readonly FakeUser Raiz = new(null, null, "Administrador");

        private static SistemaDeCadastroContext Ctx(string db, FakeUser user)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: db)
                .Options;
            return new SistemaDeCadastroContext(options, user);
        }

        private static string NewDb() => "crud_med_" + Guid.NewGuid();

        // ===== Semeadura =====

        private sealed class Seed
        {
            public long InstitutionId;
            public long DepartmentId;
            public long PatientId;
            public long EmployeeId;
            public long ClinicalConditionId;
            public long PatientClinicalConditionId;
            public long MedicineId;
            public long MpccId;
        }

        private sealed class World
        {
            public string Db = "";
            public Seed A = new();
            public Seed B = new();
        }

        // Cria duas instituicoes completas (setor, acolhido, funcionario, condicao do acolhido,
        // medicamento e medicamento programado as 07:00) e as contas de login dos usuarios.
        private static async Task<World> SeedWorld()
        {
            var w = new World { Db = NewDb() };
            using var ctx = Ctx(w.Db, Raiz);

            ctx.Institutions.AddRange(
                new Institution { Id = InstA, Name = "Inst A", Cnpj = "a", Email = "a@a.com", Phone = "1", Active = true, CreatedAt = DateTime.Now },
                new Institution { Id = InstB, Name = "Inst B", Cnpj = "b", Email = "b@b.com", Phone = "2", Active = true, CreatedAt = DateTime.Now });
            ctx.LoginAccounts.AddRange(
                new LoginAccount { Id = UserA, UserId = UserA, Name = "User A", Email = "ua@a.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = InstA },
                new LoginAccount { Id = UserB, UserId = UserB, Name = "User B", Email = "ub@b.com", PasswordHash = "x", UserType = "Colaborador", InstitutionId = InstB });

            // Catalogo de condicoes clinicas e global (sem instituicao).
            var condition = new ClinicalCondition { Name = "Hipertensao", Description = "d", Type = "Cronica" };
            ctx.ClinicalConditions.Add(condition);
            await ctx.SaveChangesAsync();

            w.A = await SeedInstitution(ctx, InstA, "a", condition.Id);
            w.B = await SeedInstitution(ctx, InstB, "b", condition.Id);
            return w;
        }

        private static async Task<Seed> SeedInstitution(SistemaDeCadastroContext ctx, long inst, string s, long conditionId)
        {
            var dep = new Department { Name = "Setor " + s, Description = "d", InstitutionId = inst };
            ctx.Departments.Add(dep);
            await ctx.SaveChangesAsync();

            var patient = new Patient
            {
                Name = "Acolhido " + s, Gender = "F", Cpf = "cpf-p-" + s, Observations = "",
                BirthDate = new DateTime(1950, 1, 1), CreatedAt = DateTime.Now, InstitutionId = inst
            };
            var employee = new Employee
            {
                Name = "Funcionario " + s, Cpf = "cpf-e-" + s, Position = "Enfermeiro", Phone = "11987654321",
                Email = $"f{s}@x.com", AdmissionDate = DateTime.Today, InstitutionId = inst, DepartmentId = dep.Id
            };
            var medicine = new Medicine
            {
                Name = "Losartana " + s, Dosage = "50mg", Description = "d", AdministrationRoute = "Oral", InstitutionId = inst
            };
            ctx.Patients.Add(patient);
            ctx.Employees.Add(employee);
            ctx.Medicines.Add(medicine);
            await ctx.SaveChangesAsync();

            var pcc = new PatientClinicalCondition
            {
                PatientId = patient.Id, ClinicalConditionId = conditionId, DiagnosisDate = DateTime.Today, Observations = ""
            };
            ctx.PatientClinicalConditions.Add(pcc);
            await ctx.SaveChangesAsync();

            var mpcc = new MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = pcc.Id, MedicineId = medicine.Id, PrescribedDosage = "1 comp",
                Frequency = "1x ao dia", AdministrationTime = new TimeSpan(7, 0, 0), ResponsibleEmployeeId = employee.Id,
                StartDate = DateTime.Today, Observations = ""
            };
            ctx.MedicinePatientClinicalConditions.Add(mpcc);
            await ctx.SaveChangesAsync();

            return new Seed
            {
                InstitutionId = inst, DepartmentId = dep.Id, PatientId = patient.Id, EmployeeId = employee.Id,
                ClinicalConditionId = conditionId, PatientClinicalConditionId = pcc.Id, MedicineId = medicine.Id, MpccId = mpcc.Id
            };
        }

        private static async Task<long> SeedAdministration(string db, Seed s, DateTime scheduled)
        {
            using var ctx = Ctx(db, Raiz);
            var ma = new MedicationAdministration
            {
                MedicinePatientClinicalConditionId = s.MpccId, PatientId = s.PatientId, EmployeeId = s.EmployeeId,
                ScheduledDateTime = scheduled, AdministeredDateTime = scheduled, Status = "Administrado",
                Observations = "", CreatedAt = DateTime.Now
            };
            ctx.MedicationAdministrations.Add(ma);
            await ctx.SaveChangesAsync();
            return ma.Id;
        }

        private static async Task<long> SeedPatientEmployee(string db, Seed s)
        {
            using var ctx = Ctx(db, Raiz);
            var link = new PatientEmployee
            {
                PatientId = s.PatientId, EmployeeId = s.EmployeeId, ResponsibilityFunction = "Cuidador",
                StartDate = DateTime.Today
            };
            ctx.PatientEmployees.Add(link);
            await ctx.SaveChangesAsync();
            return link.Id;
        }

        private static async Task<long> SeedAppointment(string db, Seed s, long userId)
        {
            using var ctx = Ctx(db, Raiz);
            var ap = new Appointment
            {
                PatientId = s.PatientId, UserId = userId, AppointmentType = "Consulta", DateTime = DateTime.Today.AddDays(1).AddHours(10),
                Responsible = "Dr. X", Status = "Agendado", Observations = "", InstitutionId = s.InstitutionId
            };
            ctx.Appointments.Add(ap);
            await ctx.SaveChangesAsync();
            return ap.Id;
        }

        // Mensagem clara = nao e a mensagem generica de erro inesperado (excecao capturada).
        private static void AssertClearError(ApiResponse ret)
        {
            Assert.False(ret.Success, "Esperava falha, mas a operacao retornou Success = true.");
            Assert.False(string.IsNullOrWhiteSpace(ret.ErrorMessage));
            Assert.False(ret.ErrorMessage.StartsWith(ApiErrors.GenericMessage),
                "Esperava mensagem clara, veio a mensagem generica de excecao: " + ret.ErrorMessage);
        }

        // ===== Fabricas das Apps (mesmas dependencias da injecao da API) =====

        private static EmployeeApp NewEmployeeApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new EmployeeRepository(ctx), new DepartmentRepository(ctx), user);

        private static PatientEmployeeApp NewPatientEmployeeApp(SistemaDeCadastroContext ctx) =>
            new(new PatientEmployeeRepository(ctx), new PatientRepository(ctx), new EmployeeRepository(ctx));

        private static MedicineApp NewMedicineApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new MedicineRepository(ctx), user);

        private static MedicinePatientClinicalConditionApp NewMpccApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new MedicinePatientClinicalConditionRepository(ctx), new PatientRepository(ctx), new MedicineRepository(ctx),
                new ClinicalConditionRepository(ctx), new PatientClinicalConditionRepository(ctx), new EmployeeRepository(ctx),
                new MedicationAdministrationRepository(ctx), user);

        private static MedicationAdministrationApp NewAdministrationApp(SistemaDeCadastroContext ctx) =>
            new(new MedicationAdministrationRepository(ctx), new MedicinePatientClinicalConditionRepository(ctx),
                new PatientRepository(ctx), new EmployeeRepository(ctx));

        private static AppointmentApp NewAppointmentApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new AppointmentRepository(ctx), new PatientRepository(ctx), user);

        // =====================================================================
        // Funcionario (EmployeeController / EmployeeApp)
        // =====================================================================

        [Fact]
        public async Task Employee_Create_PersistsInUserInstitution()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewEmployeeApp(ctx, UsuarioA).Create(new CreateEmployeeDTO
                {
                    Name = "Joana Prado", Cpf = "529.982.247-25", Position = "Tecnica", Phone = "(11) 91234-5678",
                    Email = "joana@x.com", DepartmentId = w.A.DepartmentId
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.Employees.SingleAsync(e => e.Name == "Joana Prado");
            Assert.Equal(InstA, saved.InstitutionId);
            Assert.Equal("52998224725", saved.Cpf);
            Assert.Equal("11912345678", saved.Phone);
            Assert.Equal(w.A.DepartmentId, saved.DepartmentId);
        }

        [Fact]
        public async Task Employee_Select_GetAllGetByIdAndFilter_ReturnRow_MissingIdIsNull()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewEmployeeApp(ctx, UsuarioA);

            Assert.Contains(await app.GetAll(), e => e.Id == w.A.EmployeeId);
            Assert.Equal("Funcionario a", (await app.GetById(w.A.EmployeeId))?.Name);
            Assert.Null(await app.GetById(999999));

            var paged = await app.GetEmployeeByFilter(new EmployeeFilterDTO { Name = "Funcionario a" });
            Assert.Contains(paged.Employees, e => e.Id == w.A.EmployeeId);
        }

        [Fact]
        public async Task Employee_Update_PersistsChangedFields()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewEmployeeApp(ctx, UsuarioA).Update(new UpdateEmployeeDTO
                {
                    Id = w.A.EmployeeId, Name = "Funcionario Editado", Cpf = "529.982.247-25", Position = "Coordenador",
                    Phone = "(21) 99876-5432", Email = "novo@x.com", DepartmentId = w.A.DepartmentId
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.Employees.SingleAsync(e => e.Id == w.A.EmployeeId);
            Assert.Equal("Funcionario Editado", saved.Name);
            Assert.Equal("Coordenador", saved.Position);
            Assert.Equal("21998765432", saved.Phone);
            Assert.Equal("novo@x.com", saved.Email);
        }

        [Fact]
        public async Task Employee_Delete_RemovesRow()
        {
            var w = await SeedWorld();
            // Funcionario sem nenhum vinculo, para a exclusao simples.
            long id;
            using (var seed = Ctx(w.Db, Raiz))
            {
                var e = new Employee { Name = "Avulso", Cpf = "x", Position = "P", Phone = "0", Email = "a@x.com", AdmissionDate = DateTime.Today, InstitutionId = InstA, DepartmentId = w.A.DepartmentId };
                seed.Employees.Add(e);
                await seed.SaveChangesAsync();
                id = e.Id;
            }

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewEmployeeApp(ctx, UsuarioA).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.False(await check.Employees.AnyAsync(e => e.Id == id));
        }

        [Fact]
        public async Task Employee_Delete_MissingId_ReturnsNotFound()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var ret = await NewEmployeeApp(ctx, UsuarioA).Delete(999999);
            AssertClearError(ret);
        }

        [Fact]
        public async Task Employee_Delete_Referenced_ReturnsClearErrorAndKeepsRows()
        {
            var w = await SeedWorld();
            // O funcionario semeado ja e responsavel por um medicamento programado; ganha
            // tambem um vinculo com acolhido e uma administracao registrada.
            var linkId = await SeedPatientEmployee(w.Db, w.A);
            var maId = await SeedAdministration(w.Db, w.A, DateTime.Today.AddHours(7));

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewEmployeeApp(ctx, UsuarioA).Delete(w.A.EmployeeId);
                AssertClearError(ret);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.True(await check.Employees.AnyAsync(e => e.Id == w.A.EmployeeId));
            Assert.True(await check.PatientEmployees.AnyAsync(l => l.Id == linkId && l.EmployeeId == w.A.EmployeeId));
            Assert.True(await check.MedicationAdministrations.AnyAsync(m => m.Id == maId && m.EmployeeId == w.A.EmployeeId));
        }

        [Fact]
        public async Task Employee_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioB))
            {
                var app = NewEmployeeApp(ctx, UsuarioB);
                Assert.Null(await app.GetById(w.A.EmployeeId));
                Assert.DoesNotContain(await app.GetAll(), e => e.Id == w.A.EmployeeId);
                Assert.DoesNotContain((await app.GetEmployeeByFilter(new EmployeeFilterDTO())).Employees, e => e.Id == w.A.EmployeeId);

                var upd = await app.Update(new UpdateEmployeeDTO
                {
                    Id = w.A.EmployeeId, Name = "Hacker", Position = "X", Email = "h@x.com", DepartmentId = w.B.DepartmentId
                });
                Assert.False(upd.Success);
                await app.Delete(w.A.EmployeeId);
            }

            using var check = Ctx(w.Db, Raiz);
            var row = await check.Employees.SingleAsync(e => e.Id == w.A.EmployeeId);
            Assert.Equal("Funcionario a", row.Name);
        }

        // =====================================================================
        // Vinculo acolhido-funcionario (PatientEmployeeController / PatientEmployeeApp)
        // =====================================================================

        [Fact]
        public async Task PatientEmployee_Create_Persists()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewPatientEmployeeApp(ctx).Create(new CreatePatientEmployeeDTO
                {
                    PatientId = w.A.PatientId, EmployeeId = w.A.EmployeeId, ResponsibilityFunction = "Cuidador",
                    StartDate = new DateTime(2026, 1, 10), EndDate = new DateTime(2026, 12, 31)
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.PatientEmployees.SingleAsync();
            Assert.Equal(w.A.PatientId, saved.PatientId);
            Assert.Equal(w.A.EmployeeId, saved.EmployeeId);
            Assert.Equal("Cuidador", saved.ResponsibilityFunction);
            Assert.Equal(new DateTime(2026, 1, 10), saved.StartDate);
        }

        [Fact]
        public async Task PatientEmployee_Select_GetAllGetByIdAndFilter_ReturnRow_MissingIdIsNull()
        {
            var w = await SeedWorld();
            var linkId = await SeedPatientEmployee(w.Db, w.A);

            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewPatientEmployeeApp(ctx);

            Assert.Contains(await app.GetAll(), l => l.Id == linkId);
            Assert.Equal(w.A.EmployeeId, (await app.GetById(linkId))?.EmployeeId);
            Assert.Null(await app.GetById(999999));

            var paged = await app.GetPagedPatientEmployeeByFilter(new PatientEmployeeFilterDTO { PatientId = w.A.PatientId });
            Assert.Contains(paged.PatientEmployees, l => l.Id == linkId);
        }

        // A listagem paginada deve mostrar a funcao gravada no vinculo e permitir filtrar por funcionario.
        [Fact]
        public async Task PatientEmployee_Select_FilterList_ShowsLinkFunctionAndFiltersByEmployee()
        {
            var w = await SeedWorld();
            var linkId = await SeedPatientEmployee(w.Db, w.A); // funcao "Cuidador", cargo do funcionario "Enfermeiro"

            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewPatientEmployeeApp(ctx);

            var byPatient = await app.GetPagedPatientEmployeeByFilter(new PatientEmployeeFilterDTO { PatientId = w.A.PatientId });
            var item = Assert.Single(byPatient.PatientEmployees);
            Assert.Equal("Cuidador", item.ResponsibilityFunction);
            Assert.Equal(w.A.EmployeeId, item.EmployeeId);

            var byEmployee = await app.GetPagedPatientEmployeeByFilter(new PatientEmployeeFilterDTO { EmployeeId = w.A.EmployeeId });
            Assert.Contains(byEmployee.PatientEmployees, l => l.Id == linkId);
        }

        [Fact]
        public async Task PatientEmployee_Update_PersistsChangedFields()
        {
            var w = await SeedWorld();
            var linkId = await SeedPatientEmployee(w.Db, w.A);

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewPatientEmployeeApp(ctx).Update(new UpdatePatientEmployeeDTO
                {
                    Id = linkId, PatientId = w.A.PatientId, EmployeeId = w.A.EmployeeId, ResponsibilityFunction = "Referencia",
                    StartDate = new DateTime(2026, 2, 1), EndDate = new DateTime(2026, 6, 30)
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.PatientEmployees.SingleAsync(l => l.Id == linkId);
            Assert.Equal("Referencia", saved.ResponsibilityFunction);
            Assert.Equal(new DateTime(2026, 2, 1), saved.StartDate);
            Assert.Equal(new DateTime(2026, 6, 30), saved.EndDate);
        }

        [Fact]
        public async Task PatientEmployee_Delete_RemovesRow()
        {
            var w = await SeedWorld();
            var linkId = await SeedPatientEmployee(w.Db, w.A);

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewPatientEmployeeApp(ctx).Delete(linkId);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.False(await check.PatientEmployees.AnyAsync(l => l.Id == linkId));
        }

        [Fact]
        public async Task PatientEmployee_Delete_MissingId_ReturnsNotFound()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var ret = await NewPatientEmployeeApp(ctx).Delete(999999);
            AssertClearError(ret);
        }

        [Fact]
        public async Task PatientEmployee_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var w = await SeedWorld();
            var linkId = await SeedPatientEmployee(w.Db, w.A);

            using (var ctx = Ctx(w.Db, UsuarioB))
            {
                var app = NewPatientEmployeeApp(ctx);
                Assert.Null(await app.GetById(linkId));
                Assert.DoesNotContain(await app.GetAll(), l => l.Id == linkId);
                Assert.DoesNotContain((await app.GetPagedPatientEmployeeByFilter(new PatientEmployeeFilterDTO())).PatientEmployees, l => l.Id == linkId);

                var upd = await app.Update(new UpdatePatientEmployeeDTO
                {
                    Id = linkId, PatientId = w.B.PatientId, EmployeeId = w.B.EmployeeId, ResponsibilityFunction = "Hacker"
                });
                Assert.False(upd.Success);
                await app.Delete(linkId);
            }

            using var check = Ctx(w.Db, Raiz);
            var row = await check.PatientEmployees.SingleAsync(l => l.Id == linkId);
            Assert.Equal("Cuidador", row.ResponsibilityFunction);
            Assert.Equal(w.A.PatientId, row.PatientId);
        }

        // =====================================================================
        // Medicamento (MedicineController / MedicineApp)
        // =====================================================================

        [Fact]
        public async Task Medicine_Create_PersistsInUserInstitution()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewMedicineApp(ctx, UsuarioA).CreateMedicine(new CreateMedicineDTO
                {
                    Name = "Dipirona", Dosage = "500mg", Description = "Analgesico", AdministrationRoute = "Oral"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.Medicines.SingleAsync(m => m.Name == "Dipirona");
            Assert.Equal(InstA, saved.InstitutionId);
            Assert.Equal("500mg", saved.Dosage);
            Assert.Equal("Oral", saved.AdministrationRoute);
        }

        [Fact]
        public async Task Medicine_Select_GetAllGetByIdAndFilter_ReturnRow_MissingIdIsEmpty()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewMedicineApp(ctx, UsuarioA);

            Assert.Contains(await app.GetAll(), m => m.Id == w.A.MedicineId);
            Assert.Equal("Losartana a", Assert.Single(await app.GetMedicineId(w.A.MedicineId)).Name);
            Assert.Empty(await app.GetMedicineId(999999));

            var paged = await app.GetMedicineByFilter(new MedicineFilterDTO { Name = "losartana" });
            Assert.Contains(paged.Medicines, m => m.Id == w.A.MedicineId);
        }

        [Fact]
        public async Task Medicine_Update_PersistsChangedFields()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewMedicineApp(ctx, UsuarioA).UpdateMedicine(new UpdateMedicineDTO
                {
                    Id = w.A.MedicineId, Name = "Losartana Potassica", Dosage = "100mg", Description = "Anti-hipertensivo", AdministrationRoute = "Sublingual"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.Medicines.SingleAsync(m => m.Id == w.A.MedicineId);
            Assert.Equal("Losartana Potassica", saved.Name);
            Assert.Equal("100mg", saved.Dosage);
            Assert.Equal("Anti-hipertensivo", saved.Description);
            Assert.Equal("Sublingual", saved.AdministrationRoute);
            Assert.Equal(InstA, saved.InstitutionId);
        }

        [Fact]
        public async Task Medicine_Delete_RemovesRow_AndMissingIdReturnsNotFound()
        {
            var w = await SeedWorld();
            long id;
            using (var seed = Ctx(w.Db, Raiz))
            {
                var m = new Medicine { Name = "Sem uso", Dosage = "1", Description = "", AdministrationRoute = "Oral", InstitutionId = InstA };
                seed.Medicines.Add(m);
                await seed.SaveChangesAsync();
                id = m.Id;
            }

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var app = NewMedicineApp(ctx, UsuarioA);
                var ret = await app.DeleteMedicine(new MedicineDTO { Id = id });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.DeleteMedicine(new MedicineDTO { Id = 999999 });
                AssertClearError(missing);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.False(await check.Medicines.AnyAsync(m => m.Id == id));
        }

        [Fact]
        public async Task Medicine_Delete_Referenced_ReturnsClearErrorAndKeepsRows()
        {
            var w = await SeedWorld();
            // O medicamento semeado esta em uso por um medicamento programado.
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewMedicineApp(ctx, UsuarioA).DeleteMedicine(new MedicineDTO { Id = w.A.MedicineId });
                AssertClearError(ret);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.True(await check.Medicines.AnyAsync(m => m.Id == w.A.MedicineId));
            Assert.True(await check.MedicinePatientClinicalConditions.AnyAsync(m => m.Id == w.A.MpccId && m.MedicineId == w.A.MedicineId));
        }

        [Fact]
        public async Task Medicine_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioB))
            {
                var app = NewMedicineApp(ctx, UsuarioB);
                Assert.Empty(await app.GetMedicineId(w.A.MedicineId));
                Assert.DoesNotContain(await app.GetAll(), m => m.Id == w.A.MedicineId);
                Assert.DoesNotContain((await app.GetMedicineByFilter(new MedicineFilterDTO())).Medicines, m => m.Id == w.A.MedicineId);

                var upd = await app.UpdateMedicine(new UpdateMedicineDTO { Id = w.A.MedicineId, Name = "Hacker", Dosage = "x", AdministrationRoute = "x" });
                Assert.False(upd.Success);
                var del = await app.DeleteMedicine(new MedicineDTO { Id = w.A.MedicineId });
                Assert.False(del.Success);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.Equal("Losartana a", (await check.Medicines.SingleAsync(m => m.Id == w.A.MedicineId)).Name);
        }

        // =====================================================================
        // Medicamento programado (MedicinePatientClinicalConditionController / App)
        // =====================================================================

        private static CreateMedicinePatientClinicalConditionDTO NewScheduleDto(Seed s, TimeSpan time) => new()
        {
            PatientId = s.PatientId,
            MedicineDTO = new MedicineDTO { Id = s.MedicineId },
            ClinicalConditionDTO = new ClinicalConditionDTO { Name = "Hipertensao" },
            PatientClinicalConditionDTO = new PatientClinicalConditionDTO { Id = s.PatientClinicalConditionId, PatientId = s.PatientId, Observations = "" },
            ResponsibleEmployeeId = s.EmployeeId,
            PrescribedDosage = "2 comp",
            Frequency = "2x ao dia",
            AdministrationTime = time,
            StartDate = new DateTime(2026, 3, 1),
            EndDate = new DateTime(2026, 9, 1),
            Observations = "Apos o cafe"
        };

        [Fact]
        public async Task ScheduledMedicine_Create_Persists_AndBlocksSameMedicineSameTime()
        {
            var w = await SeedWorld();
            long newId;
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var app = NewMpccApp(ctx, UsuarioA);
                var ret = await app.Create(NewScheduleDto(w.A, new TimeSpan(20, 0, 0)));
                Assert.True(ret.Success, ret.ErrorMessage);
                newId = Assert.IsType<long>(ret.Data);

                // Mesmo medicamento, mesma condicao, mesmo horario: bloqueado.
                var dup = await app.Create(NewScheduleDto(w.A, new TimeSpan(20, 0, 0)));
                Assert.False(dup.Success);
                Assert.True(dup.IsBusinessExcpetion);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.MedicinePatientClinicalConditions.SingleAsync(m => m.Id == newId);
            Assert.Equal(w.A.MedicineId, saved.MedicineId);
            Assert.Equal(w.A.PatientClinicalConditionId, saved.PatientClinicalConditionId);
            Assert.Equal("2 comp", saved.PrescribedDosage);
            Assert.Equal(new TimeSpan(20, 0, 0), saved.AdministrationTime);
            Assert.Equal(w.A.EmployeeId, saved.ResponsibleEmployeeId);
            Assert.Equal(1, await check.MedicinePatientClinicalConditions.CountAsync(m => m.AdministrationTime == new TimeSpan(20, 0, 0)));
        }

        [Fact]
        public async Task ScheduledMedicine_Select_GetAllGetByIdAndFilters_ReturnRow_MissingIdIsNull()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewMpccApp(ctx, UsuarioA);

            Assert.Contains(await app.GetAll(), m => m.Id == w.A.MpccId);
            Assert.Equal(w.A.MedicineId, (await app.GetById(w.A.MpccId))?.MedicineId);
            Assert.Null(await app.GetById(999999));
            Assert.Contains(await app.GetByPatientClinicalConditionId(w.A.PatientClinicalConditionId), m => m.Id == w.A.MpccId);

            var paged = await app.GetMedicinePatientClinicalConditionByFilter(new MedicinePatientClinicalConditionFilterDTO { PatientId = w.A.PatientId });
            var item = Assert.Single(paged.MedicinePatientClinicalConditions);
            Assert.Equal(w.A.MpccId, item.Id);
            Assert.Equal("Losartana a", item.MedicineName);
            Assert.Equal("Hipertensao", item.ClinicalConditionName);
        }

        [Fact]
        public async Task ScheduledMedicine_Update_PersistsChangedFields()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewMpccApp(ctx, UsuarioA).Update(new UpdateMedicinePatientClinicalConditionDTO
                {
                    Id = w.A.MpccId, MedicineId = w.A.MedicineId, PatientClinicalConditionId = w.A.PatientClinicalConditionId,
                    PrescribedDosage = "3 comp", Frequency = "3x ao dia", AdministrationTime = new TimeSpan(9, 15, 0),
                    ResponsibleEmployeeId = w.A.EmployeeId, StartDate = new DateTime(2026, 4, 1), EndDate = null, Observations = "Editado"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.MedicinePatientClinicalConditions.SingleAsync(m => m.Id == w.A.MpccId);
            Assert.Equal("3 comp", saved.PrescribedDosage);
            Assert.Equal("3x ao dia", saved.Frequency);
            Assert.Equal(new TimeSpan(9, 15, 0), saved.AdministrationTime);
            Assert.Equal("Editado", saved.Observations);
            Assert.Equal(new DateTime(2026, 4, 1), saved.StartDate);
        }

        [Fact]
        public async Task ScheduledMedicine_Delete_RemovesRow()
        {
            var w = await SeedWorld();
            // O programado semeado nao tem administracoes: exclusao simples.
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewMpccApp(ctx, UsuarioA).Delete(w.A.MpccId);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.False(await check.MedicinePatientClinicalConditions.AnyAsync(m => m.Id == w.A.MpccId));
        }

        [Fact]
        public async Task ScheduledMedicine_Delete_MissingId_ReturnsNotFound()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var ret = await NewMpccApp(ctx, UsuarioA).Delete(999999);
            AssertClearError(ret);
        }

        [Fact]
        public async Task ScheduledMedicine_Delete_WithAdministrations_ReturnsClearErrorOrCascades()
        {
            var w = await SeedWorld();
            var maId = await SeedAdministration(w.Db, w.A, DateTime.Today.AddHours(7));

            ApiResponse ret;
            using (var ctx = Ctx(w.Db, UsuarioA))
                ret = await NewMpccApp(ctx, UsuarioA).Delete(w.A.MpccId);

            using var check = Ctx(w.Db, Raiz);
            var mpccExists = await check.MedicinePatientClinicalConditions.AnyAsync(m => m.Id == w.A.MpccId);
            var maExists = await check.MedicationAdministrations.AnyAsync(m => m.Id == maId);

            if (ret.Success)
            {
                // Cascata: o programado e suas administracoes saem juntos (sem orfaos).
                Assert.False(mpccExists);
                Assert.False(maExists, "Programado excluido mas a administracao ficou orfa (FK quebrada).");
            }
            else
            {
                // Bloqueio: mensagem clara e nada apagado.
                AssertClearError(ret);
                Assert.True(mpccExists);
                Assert.True(maExists);
            }
        }

        [Fact]
        public async Task ScheduledMedicine_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var w = await SeedWorld();
            using (var ctx = Ctx(w.Db, UsuarioB))
            {
                var app = NewMpccApp(ctx, UsuarioB);
                Assert.Null(await app.GetById(w.A.MpccId));
                Assert.DoesNotContain(await app.GetAll(), m => m.Id == w.A.MpccId);
                Assert.Empty(await app.GetByPatientClinicalConditionId(w.A.PatientClinicalConditionId));
                Assert.DoesNotContain((await app.GetMedicinePatientClinicalConditionByFilter(new MedicinePatientClinicalConditionFilterDTO())).MedicinePatientClinicalConditions,
                    m => m.Id == w.A.MpccId);

                var upd = await app.Update(new UpdateMedicinePatientClinicalConditionDTO
                {
                    Id = w.A.MpccId, MedicineId = w.B.MedicineId, PatientClinicalConditionId = w.B.PatientClinicalConditionId,
                    PrescribedDosage = "hack", Frequency = "hack", Observations = "hack"
                });
                Assert.False(upd.Success);

                // Tambem nao pode programar medicamento para o acolhido da instituicao 1.
                var create = await app.Create(NewScheduleDto(w.A, new TimeSpan(22, 0, 0)));
                Assert.False(create.Success);

                await app.Delete(w.A.MpccId);
            }

            using var check = Ctx(w.Db, Raiz);
            var row = await check.MedicinePatientClinicalConditions.SingleAsync(m => m.Id == w.A.MpccId);
            Assert.Equal("1 comp", row.PrescribedDosage);
            Assert.False(await check.MedicinePatientClinicalConditions.AnyAsync(m => m.AdministrationTime == new TimeSpan(22, 0, 0)));
        }

        // =====================================================================
        // Administracao de medicamento (MedicationAdministrationController / App)
        // =====================================================================

        private static CreateMedicationAdministrationDTO NewAdministrationDto(Seed s, DateTime scheduled) => new()
        {
            MedicinePatientClinicalConditionId = s.MpccId,
            PatientId = s.PatientId,
            EmployeeId = s.EmployeeId,
            ScheduledDateTime = scheduled,
            AdministeredDateTime = scheduled.AddMinutes(5),
            Status = "Administrado",
            Observations = "Tomou com agua"
        };

        [Fact]
        public async Task MedicationAdministration_Create_Persists_AndBlocksDuplicateSameMinute()
        {
            var w = await SeedWorld();
            var scheduled = DateTime.Today.AddHours(9).AddMinutes(30);

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var app = NewAdministrationApp(ctx);
                var ret = await app.Create(NewAdministrationDto(w.A, scheduled));
                Assert.True(ret.Success, ret.ErrorMessage);

                // Mesmo programado + mesmo acolhido no mesmo minuto (40s depois): bloqueado.
                var dup = await app.Create(NewAdministrationDto(w.A, scheduled.AddSeconds(40)));
                Assert.False(dup.Success);
                Assert.True(dup.IsBusinessExcpetion);
                Assert.Equal("Já existe esse medicamento cadastrado nesse horário.", dup.ErrorMessage);

                // Minuto seguinte e permitido.
                var next = await app.Create(NewAdministrationDto(w.A, scheduled.AddMinutes(1)));
                Assert.True(next.Success, next.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var rows = await check.MedicationAdministrations.Where(m => m.PatientId == w.A.PatientId).OrderBy(m => m.ScheduledDateTime).ToListAsync();
            Assert.Equal(2, rows.Count);
            var saved = rows[0];
            Assert.Equal(w.A.MpccId, saved.MedicinePatientClinicalConditionId);
            Assert.Equal(w.A.EmployeeId, saved.EmployeeId);
            Assert.Equal(scheduled, saved.ScheduledDateTime);
            Assert.Equal(scheduled.AddMinutes(5), saved.AdministeredDateTime);
            Assert.Equal("Administrado", saved.Status);
            Assert.Equal("Tomou com agua", saved.Observations);
        }

        [Fact]
        public async Task MedicationAdministration_Select_GetByIdAndFilter_ReturnRow_MissingIdIsEmpty()
        {
            var w = await SeedWorld();
            var maId = await SeedAdministration(w.Db, w.A, DateTime.Today.AddHours(7));

            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewAdministrationApp(ctx);

            Assert.Equal(w.A.PatientId, Assert.Single(await app.GetById(maId)).PatientId);
            Assert.Empty(await app.GetById(999999));

            var paged = await app.GetMedicationAdministrationByFilter(new MedicationAdministrationFilterDTO { PatientId = w.A.PatientId });
            var item = Assert.Single(paged.MedicationAdministration);
            Assert.Equal(maId, item.Id);
            Assert.Equal("Losartana a", item.MedicineName);
            Assert.Equal("Funcionario a", item.EmployeeName);

            var byMedicine = await app.GetMedicationAdministrationByFilter(new MedicationAdministrationFilterDTO { MedicineId = w.A.MedicineId });
            Assert.Contains(byMedicine.MedicationAdministration, m => m.Id == maId);
        }

        [Fact]
        public async Task MedicationAdministration_Update_PersistsChangedFields()
        {
            var w = await SeedWorld();
            var maId = await SeedAdministration(w.Db, w.A, DateTime.Today.AddHours(7));
            var newScheduled = DateTime.Today.AddHours(8);

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewAdministrationApp(ctx).Update(new UpdateMedicationAdministrationDTO
                {
                    Id = maId, MedicinePatientClinicalConditionId = w.A.MpccId, PatientId = w.A.PatientId, EmployeeId = w.A.EmployeeId,
                    ScheduledDateTime = newScheduled, AdministeredDateTime = newScheduled.AddMinutes(20),
                    Status = "Atrasado", Observations = "Atrasou 20 min"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.MedicationAdministrations.SingleAsync(m => m.Id == maId);
            Assert.Equal(newScheduled, saved.ScheduledDateTime);
            Assert.Equal(newScheduled.AddMinutes(20), saved.AdministeredDateTime);
            Assert.Equal("Atrasado", saved.Status);
            Assert.Equal("Atrasou 20 min", saved.Observations);
        }

        [Fact]
        public async Task MedicationAdministration_Delete_RemovesRow_AndMissingIdReturnsNotFound()
        {
            var w = await SeedWorld();
            var maId = await SeedAdministration(w.Db, w.A, DateTime.Today.AddHours(7));

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var app = NewAdministrationApp(ctx);
                var ret = await app.Delete(maId);
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Delete(999999);
                AssertClearError(missing);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.False(await check.MedicationAdministrations.AnyAsync(m => m.Id == maId));
        }

        [Fact]
        public async Task MedicationAdministration_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var w = await SeedWorld();
            var maId = await SeedAdministration(w.Db, w.A, DateTime.Today.AddHours(7));

            using (var ctx = Ctx(w.Db, UsuarioB))
            {
                var app = NewAdministrationApp(ctx);
                Assert.Empty(await app.GetById(maId));
                Assert.DoesNotContain((await app.GetMedicationAdministrationByFilter(new MedicationAdministrationFilterDTO())).MedicationAdministration,
                    m => m.Id == maId);

                var upd = await app.Update(new UpdateMedicationAdministrationDTO
                {
                    Id = maId, MedicinePatientClinicalConditionId = w.B.MpccId, PatientId = w.B.PatientId, EmployeeId = w.B.EmployeeId,
                    ScheduledDateTime = DateTime.Today, Status = "Cancelado", Observations = "hack"
                });
                Assert.False(upd.Success);

                // Nem registrar dose para o acolhido da instituicao 1.
                var create = await app.Create(NewAdministrationDto(w.A, DateTime.Today.AddHours(11)));
                Assert.False(create.Success);

                var del = await app.Delete(maId);
                Assert.False(del.Success);
            }

            using var check = Ctx(w.Db, Raiz);
            var row = await check.MedicationAdministrations.SingleAsync(m => m.Id == maId);
            Assert.Equal("Administrado", row.Status);
            Assert.Equal(1, await check.MedicationAdministrations.CountAsync());
        }

        // =====================================================================
        // Agendamento (AppointmentController / AppointmentApp)
        // =====================================================================

        [Fact]
        public async Task Appointment_Create_PersistsInUserInstitution()
        {
            var w = await SeedWorld();
            var when = new DateTime(2026, 12, 5, 14, 30, 0);
            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewAppointmentApp(ctx, UsuarioA).Create(new CreateAppointmentDTO
                {
                    UserId = UserA, PatientId = w.A.PatientId, AppointmentType = "Fisioterapia", DateTime = when,
                    Responsible = "Dra. Ana", Status = "Agendado", Observations = "Levar exames"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.Appointments.SingleAsync();
            Assert.Equal(InstA, saved.InstitutionId);
            Assert.Equal(w.A.PatientId, saved.PatientId);
            Assert.Equal(UserA, saved.UserId);
            Assert.Equal("Fisioterapia", saved.AppointmentType);
            Assert.Equal(when, saved.DateTime);
            Assert.Equal("Dra. Ana", saved.Responsible);
        }

        [Fact]
        public async Task Appointment_Select_GetAllGetByIdAndFilter_ReturnRow_MissingIdIsNull()
        {
            var w = await SeedWorld();
            var apId = await SeedAppointment(w.Db, w.A, UserA);

            using var ctx = Ctx(w.Db, UsuarioA);
            var app = NewAppointmentApp(ctx, UsuarioA);

            var all = await app.GetAllAppointment();
            Assert.Contains(all, a => a.Id == apId && a.PatientName == "Acolhido a");

            var byId = await app.GetById(apId);
            Assert.NotNull(byId);
            Assert.Equal("Consulta", byId!.AppointmentType);
            Assert.Equal("Acolhido a", byId.PatientName);
            Assert.Null(await app.GetById(999999));

            var paged = await app.GetPagedAppointmentByFilter(new AppointmentFilterDTO { PatientId = w.A.PatientId });
            Assert.Contains(paged.Appointments, a => a.Id == apId);
        }

        [Fact]
        public async Task Appointment_Update_PersistsChangedFields()
        {
            var w = await SeedWorld();
            var apId = await SeedAppointment(w.Db, w.A, UserA);
            var when = new DateTime(2026, 12, 20, 9, 0, 0);

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewAppointmentApp(ctx, UsuarioA).Update(new UpdateAppointmentDTO
                {
                    Id = apId, PatientId = w.A.PatientId, UserId = UserA, AppointmentType = "Retorno", DateTime = when,
                    Responsible = "Dr. Y", Status = "Confirmado", Observations = "Jejum"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            var saved = await check.Appointments.SingleAsync(a => a.Id == apId);
            Assert.Equal("Retorno", saved.AppointmentType);
            Assert.Equal(when, saved.DateTime);
            Assert.Equal("Dr. Y", saved.Responsible);
            Assert.Equal("Confirmado", saved.Status);
            Assert.Equal("Jejum", saved.Observations);
            Assert.Equal(InstA, saved.InstitutionId);
            Assert.Equal(UserA, saved.UserId);
        }

        [Fact]
        public async Task Appointment_Delete_RemovesRow()
        {
            var w = await SeedWorld();
            var apId = await SeedAppointment(w.Db, w.A, UserA);

            using (var ctx = Ctx(w.Db, UsuarioA))
            {
                var ret = await NewAppointmentApp(ctx, UsuarioA).Delete(apId);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = Ctx(w.Db, Raiz);
            Assert.False(await check.Appointments.AnyAsync(a => a.Id == apId));
        }

        [Fact]
        public async Task Appointment_Delete_MissingId_ReturnsNotFound()
        {
            var w = await SeedWorld();
            using var ctx = Ctx(w.Db, UsuarioA);
            var ret = await NewAppointmentApp(ctx, UsuarioA).Delete(999999);
            AssertClearError(ret);
        }

        [Fact]
        public async Task Appointment_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var w = await SeedWorld();
            var apId = await SeedAppointment(w.Db, w.A, UserA);

            using (var ctx = Ctx(w.Db, UsuarioB))
            {
                var app = NewAppointmentApp(ctx, UsuarioB);
                Assert.Null(await app.GetById(apId));
                Assert.DoesNotContain(await app.GetAllAppointment(), a => a.Id == apId);
                Assert.DoesNotContain((await app.GetPagedAppointmentByFilter(new AppointmentFilterDTO())).Appointments, a => a.Id == apId);

                var upd = await app.Update(new UpdateAppointmentDTO
                {
                    Id = apId, PatientId = w.B.PatientId, UserId = UserB, AppointmentType = "Hack", DateTime = DateTime.Today, Status = "Cancelado"
                });
                Assert.False(upd.Success);

                // Nem agendar para o acolhido da instituicao 1.
                var create = await app.Create(new CreateAppointmentDTO
                {
                    UserId = UserB, PatientId = w.A.PatientId, AppointmentType = "Hack", DateTime = DateTime.Today, Status = "Agendado"
                });
                Assert.False(create.Success);

                await app.Delete(apId);
            }

            using var check = Ctx(w.Db, Raiz);
            var row = await check.Appointments.SingleAsync(a => a.Id == apId);
            Assert.Equal("Consulta", row.AppointmentType);
            Assert.Equal(1, await check.Appointments.CountAsync());
        }
    }
}
