using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;

namespace SistemadeCadastro.Test
{
    public class MedicationAdministrationRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName, long? institutionId = null)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new SistemaDeCadastroContext(options, new TestCurrentUserServiceWithId(institutionId));
        }

        private class TestCurrentUserServiceWithId : SistemaDeCadastro.APP.Interface.ICurrentUserServiceContext
        {
            private readonly long? _institutionId;
            public TestCurrentUserServiceWithId(long? institutionId)
            {
                _institutionId = institutionId;
            }
            public long? InstitutionId => _institutionId;
        }

        [Fact]
        public async Task GetMedicationAdministrationByStatus_Returns_List()
        {
            using var context = CreateContext("medadmin_status", 1);
            var patient = new Patient { Name = "PX", Gender = "M", Cpf = "c", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now, InstitutionId = 1 };
            context.Patients.Add(patient);
            var employee = new Employee { Name = "E1", Cpf = "111", Position = "P", Phone = "0", Email = "e@e.com", AdmissionDate = System.DateTime.Now, InstitutionId = 1 };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var ma = new MedicationAdministration { PatientId = patient.Id, EmployeeId = employee.Id, MedicinePatientClinicalConditionId = 0, ScheduledDateTime = System.DateTime.Now, Status = "Done", Observations = "obs", CreatedAt = System.DateTime.Now };
            // create a MedicinePatientClinicalCondition to satisfy FK
            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = 0, DiagnosisDate = System.DateTime.Today, Observations = "obs" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var mpcc = new SistemaDeCadastro.Domain.Models.Stage.MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = pcc.Id,
                MedicineId = 0,
                PrescribedDosage = "dos",
                Frequency = "f",
                Observations = "",
            };
            context.MedicinePatientClinicalConditions.Add(mpcc);
            await context.SaveChangesAsync();

            ma.MedicinePatientClinicalConditionId = mpcc.Id;
            context.MedicationAdministrations.Add(ma);
            await context.SaveChangesAsync();

            var repo = new MedicationAdministrationRepository(context);
            var list = await repo.GetMedicationAdministrationByStatus("Done");

            Assert.True(context.MedicationAdministrations.Any());
            Assert.NotEmpty(list);
            Assert.Equal(ma.Status, list.First().Status);
        }

        [Fact]
        public async Task GetMedicationAdministrationByFilter_Returns_Paged()
        {
            using var context = CreateContext("medadmin_filter", 1);
            var patient = new Patient { Name = "PX2", Gender = "F", Cpf = "c2", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now, InstitutionId = 1 };
            context.Patients.Add(patient);
            var employee = new Employee { Name = "E2", Cpf = "222", Position = "P2", Phone = "0", Email = "e2@e.com", AdmissionDate = System.DateTime.Now, InstitutionId = 1 };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = 0, DiagnosisDate = System.DateTime.Today, Observations = "obs" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var mpcc = new SistemaDeCadastro.Domain.Models.Stage.MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = pcc.Id,
                MedicineId = 0,
                PrescribedDosage = "dos2",
                Frequency = "f2",
                Observations = "",
            };
            context.MedicinePatientClinicalConditions.Add(mpcc);
            await context.SaveChangesAsync();

            var ma = new MedicationAdministration { PatientId = patient.Id, EmployeeId = employee.Id, MedicinePatientClinicalConditionId = mpcc.Id, ScheduledDateTime = System.DateTime.Today, Status = "Pending", Observations = "obs", CreatedAt = System.DateTime.Now };
            context.MedicationAdministrations.Add(ma);
            await context.SaveChangesAsync();

            var repo = new MedicationAdministrationRepository(context);
            var filter = new MedicationAdministrationFilterDTO { Page = 1, Status = "Pending" };
            var paged = await repo.GetMedicationAdministrationByFilter(filter);

            Assert.True(context.MedicationAdministrations.Any());
            Assert.NotEmpty(paged.MedicationAdministration);
            Assert.Equal(ma.Status, paged.MedicationAdministration.First().Status);
        }
        [Fact]
        public async Task GetMedicationAdministrationByFilter_Filters_By_Medicine_And_Period()
        {
            using var context = CreateContext("medadmin_filter_medicine_period", 1);
            var patient = new Patient { Name = "PX3", Gender = "F", Cpf = "c3", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now, InstitutionId = 1 };
            context.Patients.Add(patient);
            var employee = new Employee { Name = "E3", Cpf = "333", Position = "P3", Phone = "0", Email = "e3@e.com", AdmissionDate = System.DateTime.Now, InstitutionId = 1 };
            context.Employees.Add(employee);
            var medA = new Medicine { Name = "MedA", Description = "d", Dosage = "10", AdministrationRoute = "oral" };
            var medB = new Medicine { Name = "MedB", Description = "d", Dosage = "20", AdministrationRoute = "oral" };
            context.Medicines.AddRange(medA, medB);
            await context.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = 0, DiagnosisDate = System.DateTime.Today, Observations = "obs" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var mpccA = new MedicinePatientClinicalCondition { PatientClinicalConditionId = pcc.Id, MedicineId = medA.Id, PrescribedDosage = "10mg", Frequency = "f", Observations = "" };
            var mpccB = new MedicinePatientClinicalCondition { PatientClinicalConditionId = pcc.Id, MedicineId = medB.Id, PrescribedDosage = "20mg", Frequency = "f", Observations = "" };
            context.MedicinePatientClinicalConditions.AddRange(mpccA, mpccB);
            await context.SaveChangesAsync();

            MedicationAdministration NewAdministration(long mpccId, System.DateTime scheduled, System.DateTime? administered = null) =>
                new MedicationAdministration { PatientId = patient.Id, EmployeeId = employee.Id, MedicinePatientClinicalConditionId = mpccId, ScheduledDateTime = scheduled, AdministeredDateTime = administered, Status = "Administrado", Observations = "obs", CreatedAt = System.DateTime.Now };

            var beforePeriod = NewAdministration(mpccA.Id, new System.DateTime(2026, 10, 9, 23, 59, 0));
            var firstDay = NewAdministration(mpccA.Id, new System.DateTime(2026, 10, 10, 0, 0, 0));
            var lastDay = NewAdministration(mpccA.Id, new System.DateTime(2026, 10, 12, 23, 30, 0));
            var afterPeriod = NewAdministration(mpccA.Id, new System.DateTime(2026, 10, 13, 8, 0, 0));
            // Programada antes do periodo, mas administrada dentro dele: vale a data da administracao.
            var administeredInPeriod = NewAdministration(mpccA.Id, new System.DateTime(2026, 10, 9, 8, 0, 0), new System.DateTime(2026, 10, 11, 9, 0, 0));
            var otherMedicine = NewAdministration(mpccB.Id, new System.DateTime(2026, 10, 11, 8, 0, 0));
            context.MedicationAdministrations.AddRange(beforePeriod, firstDay, lastDay, afterPeriod, administeredInPeriod, otherMedicine);
            await context.SaveChangesAsync();

            var repo = new MedicationAdministrationRepository(context);

            var byMedicine = await repo.GetMedicationAdministrationByFilter(new MedicationAdministrationFilterDTO { Page = 1, MedicineId = medB.Id });
            Assert.Equal(new[] { otherMedicine.Id }, byMedicine.MedicationAdministration.Select(x => x.Id).ToArray());

            var byMedicineAndPeriod = await repo.GetMedicationAdministrationByFilter(new MedicationAdministrationFilterDTO
            {
                Page = 1,
                MedicineId = medA.Id,
                StartDate = new System.DateTime(2026, 10, 10),
                EndDate = new System.DateTime(2026, 10, 12)
            });
            var ids = byMedicineAndPeriod.MedicationAdministration.Select(x => x.Id).OrderBy(x => x).ToArray();
            Assert.Equal(new[] { firstDay.Id, lastDay.Id, administeredInPeriod.Id }.OrderBy(x => x).ToArray(), ids);
            Assert.Equal(3, byMedicineAndPeriod.Count);

            var onlyStart = await repo.GetMedicationAdministrationByFilter(new MedicationAdministrationFilterDTO { Page = 1, StartDate = new System.DateTime(2026, 10, 13) });
            Assert.Equal(new[] { afterPeriod.Id }, onlyStart.MedicationAdministration.Select(x => x.Id).ToArray());
        }

        [Fact]
        public async Task GetMedicineReminders_Returns_List()
        {
            using var context = CreateContext("mpcc_reminders");
            var patient = new Patient { Name = "MP1", Gender = "M", Cpf = "c", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.Add(patient);
            var med = new Medicine { Name = "MedA", Description = "d", Dosage = "10", AdministrationRoute = "oral" };
            context.Medicines.Add(med);
            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = 0, DiagnosisDate = System.DateTime.Today, Observations = "obs" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var mpcc = new MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = pcc.Id,
                MedicineId = med.Id,
                ResponsibleEmployeeId = null,
                Frequency = "daily",
                AdministrationTime = System.TimeSpan.FromHours(9),
                PrescribedDosage = "10mg",
                Observations = "",
                StartDate = System.DateTime.Today,
                EndDate = System.DateTime.Today.AddDays(1)
            };
            context.MedicinePatientClinicalConditions.Add(mpcc);
            await context.SaveChangesAsync();

            var repo = new MedicationAdministrationRepository(context);
            var reminders = await repo.GetMedicineReminders();

            Assert.NotEmpty(reminders);
        }

    }
}
