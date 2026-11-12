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
    // Regressao dos problemas encontrados na revisao de codigo: prescricao duplicada
    // ao editar o acolhido, dose "nao administrada" contando como dada e exclusao de
    // acolhido que deixava registros dependentes para tras.
    public class CodeReviewFixesTests
    {
        private const long Inst = 1;

        private class FakeUser : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            public long? InstitutionId => Inst;
            public long? UserId => null;
            public string? Role => "Administrador";
        }

        private static SistemaDeCadastroContext NewContext(string db) =>
            new(new DbContextOptionsBuilder<SistemaDeCadastroContext>().UseInMemoryDatabase(db).Options, new FakeUser());

        private static string NewDb() => "review_" + Guid.NewGuid();

        private class Seed
        {
            public long PatientId, ConditionId, MedicineId, ScheduledId;
        }

        // Um acolhido com uma condicao clinica e um medicamento programado para as 08:00.
        private static async Task<Seed> SeedPatientWithMedicine(string db)
        {
            using var ctx = NewContext(db);

            var patient = new Patient
            {
                Name = "Maria", Gender = "F", Cpf = "52998224725", Observations = "",
                BirthDate = DateTime.Today.AddYears(-80), CreatedAt = DateTime.Now, InstitutionId = Inst
            };
            var clinicalCondition = new ClinicalCondition { Name = "Hipertensão", Type = "Crônica", Description = "" };
            var medicine = new Medicine
            {
                Name = "Losartana", Dosage = "50mg", Description = "", AdministrationRoute = "Oral", InstitutionId = Inst
            };
            ctx.Patients.Add(patient);
            ctx.ClinicalConditions.Add(clinicalCondition);
            ctx.Medicines.Add(medicine);
            await ctx.SaveChangesAsync();

            var condition = new PatientClinicalCondition
            {
                PatientId = patient.Id, ClinicalConditionId = clinicalCondition.Id, Observations = ""
            };
            ctx.PatientClinicalConditions.Add(condition);
            await ctx.SaveChangesAsync();

            var scheduled = new MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = condition.Id, MedicineId = medicine.Id, PrescribedDosage = "1 comp",
                Frequency = "1x ao dia", AdministrationTime = new TimeSpan(8, 0, 0), Observations = ""
            };
            ctx.MedicinePatientClinicalConditions.Add(scheduled);
            await ctx.SaveChangesAsync();

            return new Seed
            {
                PatientId = patient.Id, ConditionId = condition.Id, MedicineId = medicine.Id, ScheduledId = scheduled.Id
            };
        }

        private static PatientApp NewPatientApp(SistemaDeCadastroContext ctx) => new(
            new PatientRepository(ctx),
            new ResponsibleRepository(ctx),
            new PatientEmployeeRepository(ctx),
            new PatientClinicalConditionRepository(ctx),
            new AppointmentRepository(ctx),
            new PaymentRepository(ctx),
            new MedicinePatientClinicalConditionRepository(ctx),
            new MedicineRepository(ctx),
            new FakeUser(),
            new EmployeeRepository(ctx));

        private static MedicationAdministrationApp NewAdministrationApp(SistemaDeCadastroContext ctx) => new(
            new MedicationAdministrationRepository(ctx),
            new MedicinePatientClinicalConditionRepository(ctx),
            new PatientRepository(ctx),
            new EmployeeRepository(ctx));

        private static UpdatePatientMedicineDTO MedicineDto(Seed s, long id, TimeSpan time, string frequency) => new()
        {
            Id = id, MedicineId = s.MedicineId, PatientClinicalConditionId = s.ConditionId,
            PrescribedDosage = "1 comp", Frequency = frequency, AdministrationTime = time, Observations = ""
        };

        // ------------------------------------------------------------------
        // Edicao do acolhido nao pode duplicar as prescricoes ja existentes
        // ------------------------------------------------------------------

        [Fact]
        public async Task DetailsPatient_ReturnsScheduledMedicinesWithPrescriptionId()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);

            using var ctx = NewContext(db);
            var details = await new PatientRepository(ctx).DetailsPatient(s.PatientId);

            var scheduled = Assert.Single(details!.ScheduledMedicines);
            Assert.Equal(s.ScheduledId, scheduled.Id);
            Assert.Equal(s.MedicineId, scheduled.MedicineId);
            Assert.Equal(s.ConditionId, scheduled.PatientClinicalConditionId);
            Assert.Equal(new TimeSpan(8, 0, 0), scheduled.AdministrationTime);
            Assert.Equal("1x ao dia", scheduled.Frequency);
        }

        [Fact]
        public async Task UpdatePatient_WithPrescriptionId_UpdatesInsteadOfDuplicating()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);

            using (var ctx = NewContext(db))
            {
                var ret = await NewPatientApp(ctx).UpdatePatient(new PatientDTO
                {
                    Id = s.PatientId,
                    ScheduledMedicines = new List<UpdatePatientMedicineDTO>
                    {
                        MedicineDto(s, s.ScheduledId, new TimeSpan(8, 0, 0), "2x ao dia")
                    }
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = NewContext(db);
            var scheduled = Assert.Single(await check.MedicinePatientClinicalConditions.ToListAsync());
            Assert.Equal("2x ao dia", scheduled.Frequency);
        }

        [Fact]
        public async Task UpdatePatient_WithoutId_SameMedicineAndTime_DoesNotDuplicate()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);

            using (var ctx = NewContext(db))
            {
                // Tela antiga (ou item reenviado) manda id 0 para uma prescricao que ja existe.
                var ret = await NewPatientApp(ctx).UpdatePatient(new PatientDTO
                {
                    Id = s.PatientId,
                    ScheduledMedicines = new List<UpdatePatientMedicineDTO>
                    {
                        MedicineDto(s, 0, new TimeSpan(8, 0, 0), "1x ao dia")
                    }
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = NewContext(db);
            Assert.Equal(1, await check.MedicinePatientClinicalConditions.CountAsync());
        }

        [Fact]
        public async Task UpdatePatient_NewMedicineAtAnotherTime_IsCreatedInTheRightCondition()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);

            using (var ctx = NewContext(db))
            {
                var ret = await NewPatientApp(ctx).UpdatePatient(new PatientDTO
                {
                    Id = s.PatientId,
                    ScheduledMedicines = new List<UpdatePatientMedicineDTO>
                    {
                        MedicineDto(s, s.ScheduledId, new TimeSpan(8, 0, 0), "1x ao dia"),
                        MedicineDto(s, 0, new TimeSpan(20, 0, 0), "1x ao dia")
                    }
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = NewContext(db);
            var all = await check.MedicinePatientClinicalConditions.ToListAsync();
            Assert.Equal(2, all.Count);
            Assert.All(all, m => Assert.Equal(s.ConditionId, m.PatientClinicalConditionId));
        }

        [Fact]
        public async Task UpdatePatient_WithPrescriptionIdOfAnotherPatient_FailsAndChangesNothing()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);

            using (var ctx = NewContext(db))
            {
                var ret = await NewPatientApp(ctx).UpdatePatient(new PatientDTO
                {
                    Id = s.PatientId,
                    Name = "Nome alterado",
                    ScheduledMedicines = new List<UpdatePatientMedicineDTO>
                    {
                        MedicineDto(s, 9999, new TimeSpan(8, 0, 0), "1x ao dia")
                    }
                });
                Assert.False(ret.Success);
            }

            using var check = NewContext(db);
            Assert.Equal("Maria", (await check.Patients.SingleAsync()).Name);
            Assert.Equal(1, await check.MedicinePatientClinicalConditions.CountAsync());
        }

        // ------------------------------------------------------------------
        // Dose "nao administrada" nao pode contar como dose dada
        // ------------------------------------------------------------------

        [Theory]
        [InlineData("Não administrado", "Não administrado")]
        [InlineData("Cancelado", "Cancelado")]
        public async Task NotAdministeredDose_HasNoAdministeredTime_AndIsNotReportedAsGiven(string status, string expectedReportStatus)
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);
            var today = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo().Date;

            using (var ctx = NewContext(db))
            {
                var ret = await NewAdministrationApp(ctx).Create(new CreateMedicationAdministrationDTO
                {
                    MedicinePatientClinicalConditionId = s.ScheduledId,
                    PatientId = s.PatientId,
                    ScheduledDateTime = today.AddHours(8),
                    AdministeredDateTime = today.AddHours(8).AddMinutes(5),
                    Status = status,
                    Observations = "Recusou"
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = NewContext(db);
            var saved = await check.MedicationAdministrations.SingleAsync();
            Assert.Null(saved.AdministeredDateTime);

            var repo = new MedicationAdministrationRepository(check);
            var item = Assert.Single(await repo.GetDailyMedicationReport(today, null));
            Assert.Equal(expectedReportStatus, item.Status);
            Assert.Null(item.AdministeredDateTime);

            // A dose ja foi tratada hoje: nao deve continuar na lista de lembretes.
            Assert.Empty(await repo.GetMedicineReminders());
        }

        [Fact]
        public async Task AdministeredDose_KeepsAdministeredTime_AndIsReportedAsGiven()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);
            var today = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo().Date;
            var givenAt = today.AddHours(8).AddMinutes(5);

            using (var ctx = NewContext(db))
            {
                var ret = await NewAdministrationApp(ctx).Create(new CreateMedicationAdministrationDTO
                {
                    MedicinePatientClinicalConditionId = s.ScheduledId,
                    PatientId = s.PatientId,
                    ScheduledDateTime = today.AddHours(8),
                    AdministeredDateTime = givenAt,
                    Status = "Administrado",
                    Observations = ""
                });
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = NewContext(db);
            Assert.Equal(givenAt, (await check.MedicationAdministrations.SingleAsync()).AdministeredDateTime);

            var item = Assert.Single(await new MedicationAdministrationRepository(check).GetDailyMedicationReport(today, null));
            Assert.Equal("Administrado", item.Status);
        }

        [Fact]
        public async Task History_IncludesAdministrationWithoutEmployee()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);
            var today = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo().Date;

            using (var ctx = NewContext(db))
            {
                await NewAdministrationApp(ctx).Create(new CreateMedicationAdministrationDTO
                {
                    MedicinePatientClinicalConditionId = s.ScheduledId, PatientId = s.PatientId, EmployeeId = null,
                    ScheduledDateTime = today.AddHours(8), Status = "Administrado", Observations = ""
                });
            }

            using var check = NewContext(db);
            var page = await new MedicationAdministrationRepository(check)
                .GetMedicationAdministrationByFilter(new SistemaDeCadastro.Domain.Filters.MedicationAdministrationFilterDTO { Page = 1 });
            Assert.Single(page.MedicationAdministration);
        }

        // ------------------------------------------------------------------
        // Excluir acolhido remove tudo o que depende dele
        // ------------------------------------------------------------------

        [Fact]
        public async Task DeletePatient_RemovesScheduledMedicinesAdministrationsAndOtherDependents()
        {
            var db = NewDb();
            var s = await SeedPatientWithMedicine(db);

            using (var ctx = NewContext(db))
            {
                ctx.Responsibles.Add(new Responsible
                {
                    PatientId = s.PatientId, Name = "Filha", Phone = "11999999999", Relationship = "Filha", Address = "Rua"
                });
                ctx.MedicationAdministrations.Add(new MedicationAdministration
                {
                    MedicinePatientClinicalConditionId = s.ScheduledId, PatientId = s.PatientId,
                    ScheduledDateTime = DateTime.Today.AddHours(8), AdministeredDateTime = DateTime.Today.AddHours(8),
                    Status = "Administrado", Observations = "", CreatedAt = DateTime.Now
                });
                var appointment = new Appointment
                {
                    PatientId = s.PatientId, UserId = 1, InstitutionId = Inst, AppointmentType = "Consulta",
                    DateTime = DateTime.Now, Responsible = "R", Status = "Agendado", Observations = ""
                };
                ctx.Appointments.Add(appointment);
                await ctx.SaveChangesAsync();
            }

            using (var ctx = NewContext(db))
            {
                var ret = await NewPatientApp(ctx).DeletePatient(s.PatientId);
                Assert.True(ret.Success, ret.ErrorMessage);
            }

            using var check = NewContext(db);
            Assert.Empty(await check.Patients.ToListAsync());
            Assert.Empty(await check.MedicationAdministrations.ToListAsync());
            Assert.Empty(await check.MedicinePatientClinicalConditions.ToListAsync());
            Assert.Empty(await check.PatientClinicalConditions.ToListAsync());
            Assert.Empty(await check.Appointments.ToListAsync());
            Assert.Empty(await check.Responsibles.ToListAsync());
        }
    }
}
