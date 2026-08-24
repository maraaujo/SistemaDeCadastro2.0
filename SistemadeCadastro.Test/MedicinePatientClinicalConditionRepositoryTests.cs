using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemadeCadastro.Test
{
    public class MedicinePatientClinicalConditionRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

    
        [Fact]
        public async Task GetByPatientClinicalConditionId_Returns_List()
        {
            using var context = CreateContext("mpcc_by_pcc");
            var patient = new Patient { Name = "MP2", Document = "d2", Phone = "0", Gender = "F", Cpf = "c2", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.Add(patient);
            var med = new Medicine { Name = "MedB", Description = "d", Dosage = "20", AdministrationRoute = "oral" };
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
                PrescribedDosage = "20mg",
                Observations = "",
                StartDate = System.DateTime.Today,
            };
            context.MedicinePatientClinicalConditions.Add(mpcc);
            await context.SaveChangesAsync();

            var repo = new MedicinePatientClinicalConditionRepository(context);
            var list = await repo.GetByPatientClinicalConditionId(pcc.Id);

            Assert.Single(list);
            Assert.Equal(med.Id, list.First().MedicineId);
        }
    }
}
