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
            var patient = new Patient { Name = "PX", Document = "d", Phone = "0", Gender = "M", Cpf = "c", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now, InstitutionId = 1 };
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
            var patient = new Patient { Name = "PX2", Document = "d2", Phone = "0", Gender = "F", Cpf = "c2", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now, InstitutionId = 1 };
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
    }
}
