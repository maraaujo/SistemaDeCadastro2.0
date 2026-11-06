using System;
using System.Collections.Generic;
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
    // Mensagens de erro sem detalhe interno, ids aninhados no cadastro de acolhido e
    // catalogo de condicoes clinicas fechado para usuarios de instituicao.
    public class SecurityHardeningTests
    {
        private const long InstA = 1;
        private const long InstB = 2;

        private class FakeUser : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            public FakeUser(long? institutionId, string? role = "Colaborador")
            {
                InstitutionId = institutionId;
                Role = role;
            }
            public long? InstitutionId { get; }
            public string? Role { get; }
        }

        private static SistemaDeCadastroContext CreateContext(string db, FakeUser user) =>
            new(new DbContextOptionsBuilder<SistemaDeCadastroContext>().UseInMemoryDatabase(db).Options, user);

        private static SistemaDeCadastroContext Unfiltered(string db) => CreateContext(db, new FakeUser(null, "Administrador"));

        private class Seed
        {
            public Patient PatientA = null!;
            public Medicine MedicineB = null!;
            public Employee EmployeeB = null!;
        }

        private static async Task<Seed> SeedData(string db)
        {
            using var ctx = Unfiltered(db);
            var s = new Seed
            {
                PatientA = new Patient
                {
                    Name = "A", Gender = "F", Cpf = "1", Observations = "", BirthDate = DateTime.Today,
                    CreatedAt = DateTime.Now, InstitutionId = InstA
                },
                MedicineB = new Medicine
                {
                    Name = "Remedio B", Dosage = "1", Description = "", AdministrationRoute = "Oral", InstitutionId = InstB
                },
                EmployeeB = new Employee
                {
                    Name = "EB", Cpf = "2", Position = "P", Phone = "0", Email = "eb@b.com",
                    AdmissionDate = DateTime.Now, InstitutionId = InstB
                }
            };
            ctx.Patients.Add(s.PatientA);
            ctx.Medicines.Add(s.MedicineB);
            ctx.Employees.Add(s.EmployeeB);
            await ctx.SaveChangesAsync();
            return s;
        }

        [Fact]
        public void ApiErrors_DoesNotExposeExceptionDetails()
        {
            var message = ApiErrors.From(new InvalidOperationException("Duplicate entry for key 'contas_login.IX_email'"));

            Assert.StartsWith(ApiErrors.GenericMessage, message);
            Assert.DoesNotContain("contas_login", message);
            Assert.DoesNotContain("Duplicate", message);
        }

        private static PatientApp NewPatientApp(SistemaDeCadastroContext ctx, FakeUser user) => new(
            new PatientRepository(ctx), new ResponsibleRepository(ctx), new PatientEmployeeRepository(ctx),
            new PatientClinicalConditionRepository(ctx), new AppointmentRepository(ctx), new PaymentRepository(ctx),
            new MedicinePatientClinicalConditionRepository(ctx), new MedicineRepository(ctx), user,
            new EmployeeRepository(ctx));

        [Fact]
        public async Task CreatePatient_WithMedicineOrEmployeeOfOtherInstitution_IsRejected()
        {
            var db = "harden_patient_" + Guid.NewGuid();
            var s = await SeedData(db);
            var user = new FakeUser(InstA);

            using (var ctx = CreateContext(db, user))
            {
                var app = NewPatientApp(ctx, user);

                var withMedicineB = await app.CreatePatient(new CreatePatientDTO
                {
                    Name = "Novo", Gender = "F",
                    ScheduledMedicines = new List<CreatePatientMedicineDTO> { new() { MedicineId = s.MedicineB.Id } }
                });
                Assert.False(withMedicineB.Success);

                var withEmployeeB = await app.CreatePatient(new CreatePatientDTO
                {
                    Name = "Novo", Gender = "F",
                    ScheduledMedicines = new List<CreatePatientMedicineDTO> { new() { ResponsibleEmployeeId = s.EmployeeB.Id } }
                });
                Assert.False(withEmployeeB.Success);
            }

            using var check = Unfiltered(db);
            Assert.Equal(1, await check.Patients.CountAsync());
            Assert.Equal(0, await check.MedicinePatientClinicalConditions.CountAsync());
        }

        [Fact]
        public async Task ScheduleMedicine_WithUnknownClinicalCondition_DoesNotGrowCatalog()
        {
            var db = "harden_catalog_" + Guid.NewGuid();
            var s = await SeedData(db);
            var user = new FakeUser(InstA);

            using (var ctx = CreateContext(db, user))
            {
                var app = new MedicinePatientClinicalConditionApp(
                    new MedicinePatientClinicalConditionRepository(ctx), new PatientRepository(ctx), new MedicineRepository(ctx),
                    new ClinicalConditionRepository(ctx), new PatientClinicalConditionRepository(ctx),
                    new EmployeeRepository(ctx), user);

                var ret = await app.Create(new CreateMedicinePatientClinicalConditionDTO
                {
                    PatientId = s.PatientA.Id,
                    MedicineDTO = new MedicineDTO { Id = 0, Name = "Remedio novo", Dosage = "1", Description = "", AdministrationRoute = "Oral" },
                    ClinicalConditionDTO = new ClinicalConditionDTO { Id = 0, Name = "Condicao inventada", Description = "", Type = "" },
                    PatientClinicalConditionDTO = new PatientClinicalConditionDTO { Id = 0, ClinicalConditionId = 0, PatientId = s.PatientA.Id },
                    PrescribedDosage = "1", Frequency = "1x"
                });

                Assert.False(ret.Success);
            }

            using var check = Unfiltered(db);
            Assert.Equal(0, await check.ClinicalConditions.CountAsync());
            Assert.Equal(1, await check.Medicines.CountAsync());
            Assert.Equal(0, await check.MedicinePatientClinicalConditions.CountAsync());
        }
    }
}
