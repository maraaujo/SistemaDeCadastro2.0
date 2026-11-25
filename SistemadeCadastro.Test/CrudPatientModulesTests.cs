using System;
using System.Collections.Generic;
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
    // Testes de CRUD ponta a ponta (App real + repositorio real + contexto em memoria) para
    // os modulos ligados ao acolhido: Patient, Responsible, BloodType, ClinicalCondition,
    // PatientClinicalCondition e Department. Cada teste responde "o CRUD funciona?":
    // grava pela App e confere o banco com um contexto novo (sem cache do ChangeTracker).
    // Quando um teste falha por bug real de producao, ele fica falhando de proposito.
    public class CrudPatientModulesTests
    {
        private const long Inst1 = 1;
        private const long Inst2 = 2;

        // Mensagem de nao encontrado: todas as Apps usam "nao encontrado(a)".
        private const string NotFoundFragment = "encontrad";

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

        // Usuario de instituicao (como o token da API para quem trabalha na instituicao).
        private static readonly FakeUser UserInst1 = new(Inst1, 10, "Colaborador");
        private static readonly FakeUser UserInst2 = new(Inst2, 20, "Colaborador");

        // Admin do SaaS (policy "SaasAdmin": Administrador sem instituicao).
        private static readonly FakeUser SaasAdmin = new(null, 1, "Administrador");

        // Usado so para semear e conferir: ve todas as instituicoes e nao gera auditoria.
        private static readonly FakeUser Root = new(null, null, "Administrador");

        private const string ValidCpf = "52998224725";
        private const string ValidCpfMasked = "529.982.247-25";
        private const string OtherValidCpf = "11144477735";
        private const string OtherValidCpfMasked = "111.444.777-35";

        private static SistemaDeCadastroContext CreateContext(string dbName, FakeUser user)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new SistemaDeCadastroContext(options, user);
        }

        private static string NewDb() => "crud_patient_" + Guid.NewGuid();

        // ===================== Fabricas das Apps (mesma montagem da injecao de dependencia) =====================

        private static PatientApp NewPatientApp(SistemaDeCadastroContext ctx, FakeUser user) => new(
            new PatientRepository(ctx),
            new ResponsibleRepository(ctx),
            new PatientEmployeeRepository(ctx),
            new PatientClinicalConditionRepository(ctx),
            new AppointmentRepository(ctx),
            new PaymentRepository(ctx),
            new MedicinePatientClinicalConditionRepository(ctx),
            new MedicineRepository(ctx),
            user,
            new EmployeeRepository(ctx));

        private static ResponsibleApp NewResponsibleApp(SistemaDeCadastroContext ctx) =>
            new(new ResponsibleRepository(ctx), new PatientRepository(ctx));

        private static BloodTypeApp NewBloodTypeApp(SistemaDeCadastroContext ctx) =>
            new(new BloodTypeRepository(ctx));

        private static ClinicalConditionApp NewClinicalConditionApp(SistemaDeCadastroContext ctx) =>
            new(new ClinicalConditionRepository(ctx));

        private static PatientClinicalConditionApp NewPatientClinicalConditionApp(SistemaDeCadastroContext ctx) =>
            new(new PatientClinicalConditionRepository(ctx),
                new PatientRepository(ctx),
                new MedicinePatientClinicalConditionRepository(ctx),
                new MedicationAdministrationRepository(ctx));

        private static DepartmentApp NewDepartmentApp(SistemaDeCadastroContext ctx, FakeUser user) =>
            new(new DepartmentRepository(ctx), user);

        // ===================== Sementes (gravadas direto no contexto, sem auditoria) =====================

        private sealed class Catalog
        {
            public long BloodTypeA { get; set; }
            public long BloodTypeO { get; set; }
            public long ConditionDiabetes { get; set; }
            public long ConditionHypertension { get; set; }
            public long MedicineMetformin { get; set; }
            public long MedicineLosartan { get; set; }
            public long Employee { get; set; }
        }

        // Catalogos globais (tipo sanguineo, condicao clinica) e itens da instituicao
        // (medicamentos e funcionario) usados pelo cadastro do acolhido.
        private static async Task<Catalog> SeedCatalog(string db, long institutionId = Inst1)
        {
            using var ctx = CreateContext(db, Root);

            var a = new BloodType { Name = "A+" };
            var o = new BloodType { Name = "O-" };
            var diabetes = new ClinicalCondition { Name = "Diabetes", Description = "Tipo 2", Type = "Cronica" };
            var hypertension = new ClinicalCondition { Name = "Hipertensao", Description = "Arterial", Type = "Cronica" };
            var metformin = new Medicine { Name = "Metformina", Dosage = "500mg", Description = "d", AdministrationRoute = "Oral", InstitutionId = institutionId };
            var losartan = new Medicine { Name = "Losartana", Dosage = "50mg", Description = "d", AdministrationRoute = "Oral", InstitutionId = institutionId };
            var employee = new Employee
            {
                Name = "Enfermeira", Cpf = "e" + institutionId, Position = "Enfermagem", Phone = "61999990000",
                Email = $"enf{institutionId}@e.com", AdmissionDate = DateTime.Today, InstitutionId = institutionId
            };

            ctx.AddRange(a, o, diabetes, hypertension, metformin, losartan, employee);
            await ctx.SaveChangesAsync();

            return new Catalog
            {
                BloodTypeA = a.Id, BloodTypeO = o.Id,
                ConditionDiabetes = diabetes.Id, ConditionHypertension = hypertension.Id,
                MedicineMetformin = metformin.Id, MedicineLosartan = losartan.Id,
                Employee = employee.Id
            };
        }

        private static async Task<long> SeedPatient(string db, long institutionId, string name = "Acolhido", string cpf = ValidCpf)
        {
            using var ctx = CreateContext(db, Root);
            var patient = new Patient
            {
                Name = name, Gender = "F", Cpf = cpf, Observations = "obs", BirthDate = new DateTime(1950, 1, 1),
                CreatedAt = DateTime.Now, InstitutionId = institutionId
            };
            ctx.Patients.Add(patient);
            await ctx.SaveChangesAsync();
            return patient.Id;
        }

        private static async Task<long> SeedResponsible(string db, long patientId, string name = "Filha")
        {
            using var ctx = CreateContext(db, Root);
            var responsible = new Responsible
            {
                PatientId = patientId, Name = name, Phone = "61999998888", Relationship = "Filha", Address = "Rua A"
            };
            ctx.Responsibles.Add(responsible);
            await ctx.SaveChangesAsync();
            return responsible.Id;
        }

        private static async Task<long> SeedPatientClinicalCondition(string db, long patientId, long clinicalConditionId)
        {
            using var ctx = CreateContext(db, Root);
            var pcc = new PatientClinicalCondition
            {
                PatientId = patientId, ClinicalConditionId = clinicalConditionId,
                DiagnosisDate = new DateTime(2020, 5, 10), Observations = "seed"
            };
            ctx.PatientClinicalConditions.Add(pcc);
            await ctx.SaveChangesAsync();
            return pcc.Id;
        }

        private static async Task<long> SeedDepartment(string db, long institutionId, string name = "Enfermaria")
        {
            using var ctx = CreateContext(db, Root);
            var department = new Department { Name = name, Description = "Setor", InstitutionId = institutionId };
            ctx.Departments.Add(department);
            await ctx.SaveChangesAsync();
            return department.Id;
        }

        // Payload de cadastro como o front (Patients/Create.vue) envia: CPF e telefone com
        // mascara, um responsavel, uma condicao e um medicamento programado.
        private static CreatePatientDTO NewPatientPayload(Catalog c, long? responsibleEmployeeId) => new()
        {
            Id = 0,
            Name = "Maria da Silva",
            BirthDate = new DateTime(1945, 3, 20),
            Gender = "F",
            Cpf = ValidCpfMasked,
            Observations = "Chegou em 2024",
            BloodTypeId = c.BloodTypeA,
            Responsibles = new List<CreateResponsibleDTO>
            {
                new() { Name = "Joana", Phone = "(61) 99999-8888", Relationship = "Filha", Address = "Rua A, 10" }
            },
            ClinicalConditions = new List<CreatePatientClinicalConditionDTO>
            {
                new() { ClinicalConditionId = c.ConditionDiabetes, DiagnosisDate = new DateTime(2019, 1, 1), Observations = "Controlada" }
            },
            ScheduledMedicines = new List<CreatePatientMedicineDTO>
            {
                new()
                {
                    Id = 0, MedicineId = c.MedicineMetformin, PatientClinicalConditionId = 0,
                    ClinicalConditionId = c.ConditionDiabetes, PrescribedDosage = "500mg", Frequency = "2x ao dia",
                    AdministrationTime = new TimeSpan(8, 0, 0), ResponsibleEmployeeId = responsibleEmployeeId,
                    StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2030, 1, 1, 23, 59, 59), Observations = "Apos o cafe"
                }
            }
        };

        private static async Task<long> CreatePatientThroughApp(string db, Catalog c)
        {
            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientApp(ctx, UserInst1).CreatePatient(NewPatientPayload(c, c.Employee));
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            return (await check.Patients.SingleAsync(p => p.Name == "Maria da Silva")).Id;
        }

        // ============================================================================================
        // PATIENT
        // ============================================================================================

        [Fact]
        public async Task Patient_Create_PersistsPatientAndRelatedLists()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientApp(ctx, UserInst1).CreatePatient(NewPatientPayload(c, c.Employee));
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var patient = await check.Patients.SingleAsync();
            Assert.Equal("Maria da Silva", patient.Name);
            Assert.Equal(Inst1, patient.InstitutionId);
            Assert.Equal(ValidCpf, patient.Cpf); // grava so os digitos
            Assert.Equal(c.BloodTypeA, patient.BloodTypeId);

            var responsible = await check.Responsibles.SingleAsync();
            Assert.Equal(patient.Id, responsible.PatientId);
            Assert.Equal("61999998888", responsible.Phone);

            var condition = await check.PatientClinicalConditions.SingleAsync();
            Assert.Equal(patient.Id, condition.PatientId);
            Assert.Equal(c.ConditionDiabetes, condition.ClinicalConditionId);

            var medicine = await check.MedicinePatientClinicalConditions.SingleAsync();
            Assert.Equal(condition.Id, medicine.PatientClinicalConditionId);
            Assert.Equal(c.MedicineMetformin, medicine.MedicineId);
            Assert.Equal(c.Employee, medicine.ResponsibleEmployeeId);
            Assert.Equal(new TimeSpan(8, 0, 0), medicine.AdministrationTime);
        }

        // O front (Create.vue e update.vue) envia "responsibleEmployeeId: Number(x) || 0" quando
        // nenhum funcionario foi escolhido. O vinculo e opcional (long?) e tem FK para funcionarios:
        // 0 precisa virar null, senao o MySQL recusa a gravacao (FK para id 0 inexistente).
        [Fact]
        public async Task Patient_Create_MedicineWithoutEmployee_AsFrontSends_StoresNullEmployee()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientApp(ctx, UserInst1).CreatePatient(NewPatientPayload(c, responsibleEmployeeId: 0));
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var medicine = await check.MedicinePatientClinicalConditions.SingleAsync();
            Assert.Null(medicine.ResponsibleEmployeeId);
        }

        [Fact]
        public async Task Patient_Select_GetAllGetByIdDetailsAndFilter_ReturnCreatedRow()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var id = await CreatePatientThroughApp(db, c);

            using var ctx = CreateContext(db, UserInst1);
            var app = NewPatientApp(ctx, UserInst1);

            var all = await app.GetAllPatients();
            Assert.Contains(all, p => p.Id == id);

            var byId = Assert.Single(await app.GetPatientById(id));
            Assert.Equal("Maria da Silva", byId.Name);

            var details = await app.DetailsPatient(id);
            Assert.NotNull(details);
            Assert.Equal("Maria da Silva", details!.Patient.Name);
            Assert.Single(details.Responsibles);
            Assert.Contains(details.ClinicalConditions, x => x.Id == c.ConditionDiabetes);
            Assert.Single(details.ScheduledMedicines);
            Assert.Equal(c.BloodTypeA, details.BloodType.Id);

            var filtered = await app.FilterPatient(new PatientFilterDTO { Page = 1 });
            Assert.Contains(filtered.Patients, p => p.Id == id);

            // Id inexistente: lista vazia / null, sem excecao.
            Assert.Empty(await app.GetPatientById(999_999));
            Assert.Null(await app.DetailsPatient(999_999));
        }

        // Fluxo da tela de edicao (Patients/update.vue): carrega a ficha, altera campos, edita
        // os itens existentes (com id) e acrescenta itens novos (id 0) em cada lista.
        [Fact]
        public async Task Patient_Update_ChangesFieldsAndRelatedLists_AsFrontSends()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var id = await CreatePatientThroughApp(db, c);

            long responsibleId, conditionId, scheduledId;
            using (var load = CreateContext(db, UserInst1))
            {
                var details = await NewPatientApp(load, UserInst1).DetailsPatient(id);
                responsibleId = details!.Responsibles.Single().Id;
                scheduledId = details.ScheduledMedicines.Single().Id;
                conditionId = details.ScheduledMedicines.Single().PatientClinicalConditionId;
            }

            var payload = new PatientDTO
            {
                Id = id,
                Name = "Maria da Silva Souza",
                BirthDate = new DateTime(1946, 4, 21),
                Gender = "F",
                Cpf = OtherValidCpfMasked,
                Observations = "Observacao editada",
                BloodTypeId = c.BloodTypeO,
                Responsibles = new List<UpdateResponsibleDTO>
                {
                    new() { Id = responsibleId, PatientId = id, Name = "Joana Editada", Phone = "(61) 3333-4444", Relationship = "Filha", Address = "Rua B, 20" },
                    new() { Id = 0, PatientId = id, Name = "Pedro", Phone = "(61) 98888-7777", Relationship = "Filho", Address = "Rua C, 30" }
                },
                ClinicalConditions = new List<UpdatePatientClinicalConditionDTO>
                {
                    new() { Id = conditionId, PatientId = id, ClinicalConditionId = c.ConditionDiabetes, DiagnosisDate = new DateTime(2018, 6, 1), Observations = "Insulina" },
                    new() { Id = 0, PatientId = id, ClinicalConditionId = c.ConditionHypertension, DiagnosisDate = new DateTime(2022, 2, 2), Observations = "Nova" }
                },
                ScheduledMedicines = new List<UpdatePatientMedicineDTO>
                {
                    new()
                    {
                        Id = scheduledId, MedicineId = c.MedicineMetformin, PatientClinicalConditionId = conditionId,
                        ClinicalConditionId = c.ConditionDiabetes, PrescribedDosage = "850mg", Frequency = "3x ao dia",
                        AdministrationTime = new TimeSpan(8, 0, 0), ResponsibleEmployeeId = c.Employee,
                        StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2030, 1, 1, 23, 59, 59), Observations = "Apos o cafe"
                    },
                    new()
                    {
                        Id = 0, MedicineId = c.MedicineLosartan, PatientClinicalConditionId = 0,
                        ClinicalConditionId = c.ConditionHypertension, PrescribedDosage = "50mg", Frequency = "1x ao dia",
                        AdministrationTime = new TimeSpan(20, 0, 0), ResponsibleEmployeeId = c.Employee,
                        StartDate = new DateTime(2024, 2, 1), EndDate = null, Observations = "Noite"
                    }
                }
            };

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientApp(ctx, UserInst1).UpdatePatient(payload);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var patient = await check.Patients.SingleAsync(p => p.Id == id);
            Assert.Equal("Maria da Silva Souza", patient.Name);
            Assert.Equal(new DateTime(1946, 4, 21), patient.BirthDate);
            Assert.Equal(OtherValidCpf, patient.Cpf);
            Assert.Equal("Observacao editada", patient.Observations);
            Assert.Equal(c.BloodTypeO, patient.BloodTypeId);

            var responsibles = await check.Responsibles.Where(r => r.PatientId == id).OrderBy(r => r.Id).ToListAsync();
            Assert.Equal(2, responsibles.Count);
            Assert.Equal("Joana Editada", responsibles[0].Name);
            Assert.Equal("6133334444", responsibles[0].Phone);
            Assert.Equal("Rua B, 20", responsibles[0].Address);
            Assert.Equal("Pedro", responsibles[1].Name);
            Assert.Equal("61988887777", responsibles[1].Phone);

            var conditions = await check.PatientClinicalConditions.Where(x => x.PatientId == id).OrderBy(x => x.Id).ToListAsync();
            Assert.Equal(2, conditions.Count);
            Assert.Equal("Insulina", conditions[0].Observations);
            Assert.Equal(new DateTime(2018, 6, 1), conditions[0].DiagnosisDate);
            Assert.Equal(c.ConditionHypertension, conditions[1].ClinicalConditionId);

            var medicines = await check.MedicinePatientClinicalConditions.OrderBy(m => m.Id).ToListAsync();
            Assert.Equal(2, medicines.Count);
            Assert.Equal(scheduledId, medicines[0].Id);
            Assert.Equal("3x ao dia", medicines[0].Frequency);
            Assert.Equal("850mg", medicines[0].PrescribedDosage);
            Assert.Equal(c.MedicineLosartan, medicines[1].MedicineId);
            // O medicamento novo vai para a condicao informada (hipertensao, criada nesta edicao).
            Assert.Equal(conditions[1].Id, medicines[1].PatientClinicalConditionId);
        }

        // Na tela de edicao o usuario remove um responsavel da lista (splice no front) e salva:
        // o payload chega sem esse responsavel e o esperado e que ele deixe de existir.
        [Fact]
        public async Task Patient_Update_ResponsibleRemovedInFront_IsRemoved()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var id = await CreatePatientThroughApp(db, c);
            var extraId = await SeedResponsible(db, id, "Removido");

            long keptId;
            using (var load = CreateContext(db, Root))
                keptId = (await load.Responsibles.SingleAsync(r => r.PatientId == id && r.Name == "Joana")).Id;

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientApp(ctx, UserInst1).UpdatePatient(new PatientDTO
                {
                    Id = id,
                    Name = "Maria da Silva",
                    Responsibles = new List<UpdateResponsibleDTO>
                    {
                        new() { Id = keptId, PatientId = id, Name = "Joana", Phone = "61999998888", Relationship = "Filha", Address = "Rua A, 10" }
                    },
                    ClinicalConditions = new List<UpdatePatientClinicalConditionDTO>(),
                    ScheduledMedicines = new List<UpdatePatientMedicineDTO>()
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.Responsibles.AnyAsync(r => r.Id == extraId));
            Assert.True(await check.Responsibles.AnyAsync(r => r.Id == keptId));
        }

        [Fact]
        public async Task Patient_Update_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, UserInst1);

            var ret = await NewPatientApp(ctx, UserInst1).UpdatePatient(new PatientDTO { Id = 999_999, Name = "X" });

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage);
        }

        [Fact]
        public async Task Patient_Delete_WithDependents_RemovesPatientAndDependents()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var id = await CreatePatientThroughApp(db, c);

            // Dependentes extras: segundo responsavel e uma administracao do medicamento programado.
            await SeedResponsible(db, id, "Outro");
            using (var seed = CreateContext(db, Root))
            {
                var scheduled = await seed.MedicinePatientClinicalConditions.SingleAsync();
                seed.MedicationAdministrations.Add(new MedicationAdministration
                {
                    PatientId = id, EmployeeId = c.Employee, MedicinePatientClinicalConditionId = scheduled.Id,
                    ScheduledDateTime = DateTime.Now, Status = "Administrado", Observations = "", CreatedAt = DateTime.Now
                });
                await seed.SaveChangesAsync();
            }

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientApp(ctx, UserInst1).DeletePatient(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.Patients.AnyAsync(p => p.Id == id));
            Assert.False(await check.Responsibles.AnyAsync(r => r.PatientId == id));
            Assert.False(await check.PatientClinicalConditions.AnyAsync(x => x.PatientId == id));
            Assert.False(await check.MedicinePatientClinicalConditions.AnyAsync());
            Assert.False(await check.MedicationAdministrations.AnyAsync(a => a.PatientId == id));
        }

        [Fact]
        public async Task Patient_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, UserInst1);

            var ret = await NewPatientApp(ctx, UserInst1).DeletePatient(999_999);

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage);
        }

        [Fact]
        public async Task Patient_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var db = NewDb();
            var id = await SeedPatient(db, Inst1, "Da Instituicao 1");
            await SeedResponsible(db, id);

            using (var ctx = CreateContext(db, UserInst2))
            {
                var app = NewPatientApp(ctx, UserInst2);

                Assert.DoesNotContain(await app.GetAllPatients(), p => p.Id == id);
                Assert.Empty(await app.GetPatientById(id));
                Assert.Null(await app.DetailsPatient(id));

                var update = await app.UpdatePatient(new PatientDTO { Id = id, Name = "Invadido" });
                Assert.False(update.Success);

                var delete = await app.DeletePatient(id);
                Assert.False(delete.Success);
            }

            using var check = CreateContext(db, Root);
            var patient = await check.Patients.SingleAsync(p => p.Id == id);
            Assert.Equal("Da Instituicao 1", patient.Name);
            Assert.True(await check.Responsibles.AnyAsync(r => r.PatientId == id));
        }

        // ============================================================================================
        // RESPONSIBLE
        // ============================================================================================

        [Fact]
        public async Task Responsible_Create_PersistsRow()
        {
            var db = NewDb();
            var patientId = await SeedPatient(db, Inst1);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewResponsibleApp(ctx).Create(new CreateResponsibleDTO
                {
                    PatientId = patientId, Name = "Carlos", Phone = "(61) 99876-5432", Relationship = "Sobrinho", Address = "Rua X"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.Responsibles.SingleAsync();
            Assert.Equal(patientId, row.PatientId);
            Assert.Equal("Carlos", row.Name);
            Assert.Equal("61998765432", row.Phone);
            Assert.Equal("Sobrinho", row.Relationship);
        }

        [Fact]
        public async Task Responsible_Select_GetAllAndGetById()
        {
            var db = NewDb();
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedResponsible(db, patientId, "Joana");

            using var ctx = CreateContext(db, UserInst1);
            var app = NewResponsibleApp(ctx);

            Assert.Contains(await app.GetAll(), r => r.Id == id);
            var byId = await app.GetById(id);
            Assert.NotNull(byId);
            Assert.Equal("Joana", byId!.Name);

            Assert.Null(await app.GetById(999_999));
        }

        [Fact]
        public async Task Responsible_Update_PersistsChanges()
        {
            var db = NewDb();
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedResponsible(db, patientId);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var app = NewResponsibleApp(ctx);
                var ret = await app.Update(new UpdateResponsibleDTO
                {
                    Id = id, PatientId = patientId, Name = "Nome Novo", Phone = "(61) 3333-4444", Relationship = "Neta", Address = "Rua Nova"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdateResponsibleDTO
                {
                    Id = 999_999, PatientId = patientId, Name = "X", Phone = "", Relationship = "X", Address = "X"
                });
                Assert.False(missing.Success);
                Assert.Contains(NotFoundFragment, missing.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.Responsibles.SingleAsync(r => r.Id == id);
            Assert.Equal("Nome Novo", row.Name);
            Assert.Equal("6133334444", row.Phone);
            Assert.Equal("Neta", row.Relationship);
            Assert.Equal("Rua Nova", row.Address);
        }

        [Fact]
        public async Task Responsible_Delete_RemovesRow()
        {
            var db = NewDb();
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedResponsible(db, patientId);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewResponsibleApp(ctx).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.Responsibles.AnyAsync(r => r.Id == id));
        }

        [Fact]
        public async Task Responsible_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, UserInst1);

            var ret = await NewResponsibleApp(ctx).Delete(999_999);

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage ?? "");
        }

        [Fact]
        public async Task Responsible_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var db = NewDb();
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedResponsible(db, patientId, "Original");

            using (var ctx = CreateContext(db, UserInst2))
            {
                var app = NewResponsibleApp(ctx);

                Assert.DoesNotContain(await app.GetAll(), r => r.Id == id);
                Assert.Null(await app.GetById(id));

                var update = await app.Update(new UpdateResponsibleDTO
                {
                    Id = id, PatientId = patientId, Name = "Invadido", Phone = "", Relationship = "X", Address = "X"
                });
                Assert.False(update.Success);

                await app.Delete(id);

                // Tambem nao pode criar responsavel para acolhido de outra instituicao.
                var create = await app.Create(new CreateResponsibleDTO
                {
                    PatientId = patientId, Name = "Intruso", Phone = "", Relationship = "X", Address = "X"
                });
                Assert.False(create.Success);
            }

            using var check = CreateContext(db, Root);
            var row = await check.Responsibles.SingleAsync(r => r.PatientId == patientId);
            Assert.Equal(id, row.Id);
            Assert.Equal("Original", row.Name);
        }

        // ============================================================================================
        // BLOOD TYPE (catalogo global; controller expoe so GetAll/GetById, a App tem Create/Delete)
        // ============================================================================================

        [Fact]
        public async Task BloodType_Create_ReturnsSuccessAndPersists()
        {
            var db = NewDb();

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewBloodTypeApp(ctx).CreateBloodType(new BloodTypeDTO { Id = 0, Name = "AB+" });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.True(await check.BloodTypes.AnyAsync(b => b.Name == "AB+"));
        }

        [Fact]
        public async Task BloodType_Select_GetAllAndGetById()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            using var ctx = CreateContext(db, UserInst1);
            var app = NewBloodTypeApp(ctx);

            var all = await app.GetAllBloodIllness();
            Assert.True(all.Success, all.ErrorMessage);
            var list = Assert.IsAssignableFrom<IEnumerable<BloodType>>(all.Data);
            Assert.Contains(list, b => b.Id == c.BloodTypeA);

            var byId = Assert.Single(await app.GetBloodTypeById(c.BloodTypeA));
            Assert.Equal("A+", byId.Name);

            Assert.Empty(await app.GetBloodTypeById(999_999));
        }

        [Fact]
        public async Task BloodType_Delete_ReturnsSuccessAndRemoves()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewBloodTypeApp(ctx).DeleteIllness(new BloodTypeDTO { Id = c.BloodTypeO });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.BloodTypes.AnyAsync(b => b.Id == c.BloodTypeO));
        }

        [Fact]
        public async Task BloodType_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, SaasAdmin);

            var ret = await NewBloodTypeApp(ctx).DeleteIllness(new BloodTypeDTO { Id = 999_999 });

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage ?? "");
        }

        // ============================================================================================
        // CLINICAL CONDITION (catalogo global; Create/Update/Delete so para SaasAdmin)
        // ============================================================================================

        [Fact]
        public async Task ClinicalCondition_Create_PersistsRow()
        {
            var db = NewDb();

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewClinicalConditionApp(ctx).Create(new CreateClinicalConditionDTO
                {
                    Name = "Alzheimer", Description = "Demencia", Type = "Neurologica"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.ClinicalConditions.SingleAsync();
            Assert.Equal("Alzheimer", row.Name);
            Assert.Equal("Demencia", row.Description);
            Assert.Equal("Neurologica", row.Type);
        }

        [Fact]
        public async Task ClinicalCondition_Select_GetAllAndGetById_VisibleToInstitutionUser()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            // Catalogo global: o usuario da instituicao tambem le (combos da tela do acolhido).
            using var ctx = CreateContext(db, UserInst1);
            var app = NewClinicalConditionApp(ctx);

            Assert.Contains(await app.GetAll(), x => x.Id == c.ConditionDiabetes);
            var byId = await app.GetById(c.ConditionDiabetes);
            Assert.NotNull(byId);
            Assert.Equal("Diabetes", byId!.Name);

            Assert.Null(await app.GetById(999_999));
        }

        [Fact]
        public async Task ClinicalCondition_Update_PersistsChanges()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var app = NewClinicalConditionApp(ctx);
                var ret = await app.Update(new UpdateClinicalConditionDTO
                {
                    Id = c.ConditionDiabetes, Name = "Diabetes Mellitus", Description = "Tipo 1", Type = "Endocrina"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdateClinicalConditionDTO { Id = 999_999, Name = "X", Description = "X", Type = "X" });
                Assert.False(missing.Success);
                Assert.Contains(NotFoundFragment, missing.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.ClinicalConditions.SingleAsync(x => x.Id == c.ConditionDiabetes);
            Assert.Equal("Diabetes Mellitus", row.Name);
            Assert.Equal("Tipo 1", row.Description);
            Assert.Equal("Endocrina", row.Type);
        }

        [Fact]
        public async Task ClinicalCondition_Delete_RemovesRow()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);

            using (var ctx = CreateContext(db, SaasAdmin))
            {
                var ret = await NewClinicalConditionApp(ctx).Delete(c.ConditionHypertension);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.ClinicalConditions.AnyAsync(x => x.Id == c.ConditionHypertension));
            Assert.True(await check.ClinicalConditions.AnyAsync(x => x.Id == c.ConditionDiabetes));
        }

        [Fact]
        public async Task ClinicalCondition_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, SaasAdmin);

            var ret = await NewClinicalConditionApp(ctx).Delete(999_999);

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage ?? "");
        }

        // ============================================================================================
        // PATIENT CLINICAL CONDITION
        // ============================================================================================

        [Fact]
        public async Task PatientClinicalCondition_Create_PersistsRow()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var patientId = await SeedPatient(db, Inst1);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var app = NewPatientClinicalConditionApp(ctx);
                var ret = await app.Create(new CreatePatientClinicalConditionDTO
                {
                    PatientId = patientId, ClinicalConditionId = c.ConditionDiabetes,
                    DiagnosisDate = new DateTime(2021, 7, 7), Observations = "Inicial"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var invalid = await app.Create(new CreatePatientClinicalConditionDTO
                {
                    PatientId = 0, ClinicalConditionId = c.ConditionDiabetes, Observations = "x"
                });
                Assert.False(invalid.Success);
            }

            using var check = CreateContext(db, Root);
            var row = await check.PatientClinicalConditions.SingleAsync();
            Assert.Equal(patientId, row.PatientId);
            Assert.Equal(c.ConditionDiabetes, row.ClinicalConditionId);
            Assert.Equal(new DateTime(2021, 7, 7), row.DiagnosisDate);
            Assert.Equal("Inicial", row.Observations);
        }

        [Fact]
        public async Task PatientClinicalCondition_Select_GetAllGetByIdAndByPatient()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedPatientClinicalCondition(db, patientId, c.ConditionDiabetes);

            using var ctx = CreateContext(db, UserInst1);
            var app = NewPatientClinicalConditionApp(ctx);

            Assert.Contains(await app.GetAll(), x => x.Id == id);

            var byId = await app.GetById(id);
            Assert.NotNull(byId);
            Assert.Equal(c.ConditionDiabetes, byId!.ClinicalConditionId);

            var byPatient = Assert.Single(await app.GetPatientClinicalConditionByPatientId(patientId));
            Assert.Equal(id, byPatient.Id);
            Assert.Equal("Diabetes", byPatient.ClinicalCondition);

            Assert.Null(await app.GetById(999_999));
        }

        // A tela de edicao do acolhido chama GetPatientClinicalConditionByPatientId ao abrir;
        // acolhido sem nenhuma condicao (ou id inexistente) deve dar vazio/null, nao excecao (HTTP 500).
        [Fact]
        public async Task PatientClinicalCondition_GetByPatientId_PatientWithoutConditions_DoesNotThrow()
        {
            var db = NewDb();
            var patientId = await SeedPatient(db, Inst1);

            using var ctx = CreateContext(db, UserInst1);
            var app = NewPatientClinicalConditionApp(ctx);

            var ex = await Record.ExceptionAsync(() => app.GetPatientClinicalConditionByPatientId(patientId));
            Assert.Null(ex);
            Assert.Empty(await app.GetPatientClinicalConditionByPatientId(patientId));
        }

        [Fact]
        public async Task PatientClinicalCondition_Update_PersistsChanges()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedPatientClinicalCondition(db, patientId, c.ConditionDiabetes);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var app = NewPatientClinicalConditionApp(ctx);
                var ret = await app.Update(new PatientClinicalCondition
                {
                    Id = id, PatientId = patientId, ClinicalConditionId = c.ConditionHypertension,
                    DiagnosisDate = new DateTime(2023, 3, 3), Observations = "Editada"
                });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new PatientClinicalCondition
                {
                    Id = 999_999, PatientId = patientId, ClinicalConditionId = c.ConditionDiabetes, Observations = "x"
                });
                Assert.False(missing.Success);
                Assert.Contains(NotFoundFragment, missing.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.PatientClinicalConditions.SingleAsync(x => x.Id == id);
            Assert.Equal(c.ConditionHypertension, row.ClinicalConditionId);
            Assert.Equal(new DateTime(2023, 3, 3), row.DiagnosisDate);
            Assert.Equal("Editada", row.Observations);
        }

        [Fact]
        public async Task PatientClinicalCondition_Delete_WithMedicinesAndAdministrations_RemovesAll()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedPatientClinicalCondition(db, patientId, c.ConditionDiabetes);

            using (var seed = CreateContext(db, Root))
            {
                var scheduled = new MedicinePatientClinicalCondition
                {
                    PatientClinicalConditionId = id, MedicineId = c.MedicineMetformin, PrescribedDosage = "500mg",
                    Frequency = "1x", Observations = "", AdministrationTime = new TimeSpan(8, 0, 0)
                };
                seed.MedicinePatientClinicalConditions.Add(scheduled);
                await seed.SaveChangesAsync();
                seed.MedicationAdministrations.Add(new MedicationAdministration
                {
                    PatientId = patientId, EmployeeId = c.Employee, MedicinePatientClinicalConditionId = scheduled.Id,
                    ScheduledDateTime = DateTime.Now, Status = "Administrado", Observations = "", CreatedAt = DateTime.Now
                });
                await seed.SaveChangesAsync();
            }

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewPatientClinicalConditionApp(ctx).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.PatientClinicalConditions.AnyAsync(x => x.Id == id));
            Assert.False(await check.MedicinePatientClinicalConditions.AnyAsync());
            Assert.False(await check.MedicationAdministrations.AnyAsync());
            Assert.True(await check.Patients.AnyAsync(p => p.Id == patientId));
        }

        [Fact]
        public async Task PatientClinicalCondition_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, UserInst1);

            var ret = await NewPatientClinicalConditionApp(ctx).Delete(999_999);

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage ?? "");
        }

        [Fact]
        public async Task PatientClinicalCondition_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var db = NewDb();
            var c = await SeedCatalog(db);
            var patientId = await SeedPatient(db, Inst1);
            var id = await SeedPatientClinicalCondition(db, patientId, c.ConditionDiabetes);

            using (var ctx = CreateContext(db, UserInst2))
            {
                var app = NewPatientClinicalConditionApp(ctx);

                Assert.DoesNotContain(await app.GetAll(), x => x.Id == id);
                Assert.Null(await app.GetById(id));

                var update = await app.Update(new PatientClinicalCondition
                {
                    Id = id, PatientId = patientId, ClinicalConditionId = c.ConditionHypertension, Observations = "Invadido"
                });
                Assert.False(update.Success);

                await app.Delete(id);

                var create = await app.Create(new CreatePatientClinicalConditionDTO
                {
                    PatientId = patientId, ClinicalConditionId = c.ConditionHypertension, Observations = "Intruso"
                });
                Assert.False(create.Success);
            }

            using var check = CreateContext(db, Root);
            var row = await check.PatientClinicalConditions.SingleAsync();
            Assert.Equal(id, row.Id);
            Assert.Equal(c.ConditionDiabetes, row.ClinicalConditionId);
            Assert.Equal("seed", row.Observations);
        }

        // ============================================================================================
        // DEPARTMENT
        // ============================================================================================

        [Fact]
        public async Task Department_Create_PersistsRowInUserInstitution()
        {
            var db = NewDb();

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewDepartmentApp(ctx, UserInst1).Create(new CreateDepartmentDTO
                {
                    Name = "Fisioterapia", Description = "Reabilitacao"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.Departments.SingleAsync();
            Assert.Equal("Fisioterapia", row.Name);
            Assert.Equal("Reabilitacao", row.Description);
            Assert.Equal(Inst1, row.InstitutionId);
        }

        [Fact]
        public async Task Department_Select_GetAllGetByIdAndFilter()
        {
            var db = NewDb();
            var id = await SeedDepartment(db, Inst1, "Enfermaria");

            using var ctx = CreateContext(db, UserInst1);
            var app = NewDepartmentApp(ctx, UserInst1);

            Assert.Contains(await app.GetAll(), d => d.Id == id);

            var byId = await app.GetById(id);
            Assert.NotNull(byId);
            Assert.Equal("Enfermaria", byId!.Name);

            var filtered = await app.GetDepartmentByFilter(new SistemaDeCadastro.Domain.Filters.DepartmentFilterDTO { Name = "enfer", Page = 1 });
            Assert.Contains(filtered.Departments, d => d.Id == id);

            Assert.Null(await app.GetById(999_999));
        }

        [Fact]
        public async Task Department_Update_PersistsChanges()
        {
            var db = NewDb();
            var id = await SeedDepartment(db, Inst1);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var app = NewDepartmentApp(ctx, UserInst1);
                var ret = await app.Update(new UpdateDepartmentDTO { Id = id, Name = "Nutricao", Description = "Cozinha" });
                Assert.True(ret.Success, ret.ErrorMessage);

                var missing = await app.Update(new UpdateDepartmentDTO { Id = 999_999, Name = "X", Description = "X" });
                Assert.False(missing.Success);
                Assert.Contains(NotFoundFragment, missing.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            var row = await check.Departments.SingleAsync(d => d.Id == id);
            Assert.Equal("Nutricao", row.Name);
            Assert.Equal("Cozinha", row.Description);
            Assert.Equal(Inst1, row.InstitutionId);
        }

        [Fact]
        public async Task Department_Delete_RemovesRow()
        {
            var db = NewDb();
            var id = await SeedDepartment(db, Inst1);

            using (var ctx = CreateContext(db, UserInst1))
            {
                var ret = await NewDepartmentApp(ctx, UserInst1).Delete(id);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = CreateContext(db, Root);
            Assert.False(await check.Departments.AnyAsync(d => d.Id == id));
        }

        [Fact]
        public async Task Department_Delete_MissingId_ReturnsNotFound()
        {
            var db = NewDb();
            using var ctx = CreateContext(db, UserInst1);

            var ret = await NewDepartmentApp(ctx, UserInst1).Delete(999_999);

            Assert.False(ret.Success);
            Assert.Contains(NotFoundFragment, ret.ErrorMessage ?? "");
        }

        [Fact]
        public async Task Department_Isolation_OtherInstitutionCannotSeeUpdateOrDelete()
        {
            var db = NewDb();
            var id = await SeedDepartment(db, Inst1, "Original");

            using (var ctx = CreateContext(db, UserInst2))
            {
                var app = NewDepartmentApp(ctx, UserInst2);

                Assert.DoesNotContain(await app.GetAll(), d => d.Id == id);
                Assert.Null(await app.GetById(id));

                var update = await app.Update(new UpdateDepartmentDTO { Id = id, Name = "Invadido", Description = "X" });
                Assert.False(update.Success);

                await app.Delete(id);
            }

            using var check = CreateContext(db, Root);
            var row = await check.Departments.SingleAsync(d => d.Id == id);
            Assert.Equal("Original", row.Name);
            Assert.Equal(Inst1, row.InstitutionId);
        }
    }
}
