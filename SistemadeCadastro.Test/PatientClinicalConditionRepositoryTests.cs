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
    public class PatientClinicalConditionRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

        [Fact]
        public async Task GetPatientClinicalConditionByPatientId_Returns_DTO()
        {
            using var context = CreateContext("pcc_by_patient");
            var patient = new Patient { Name = "PCP", Document = "d", Phone = "0", Gender = "M", Cpf = "c", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.Add(patient);
            var cc = new ClinicalCondition { Name = "Cond", Type = "T", Description = "D" };
            context.ClinicalConditions.Add(cc);
            await context.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = cc.Id, DiagnosisDate = System.DateTime.Today, Observations = "obs" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var repo = new PatientClinicalConditionRepository(context);
            var dto = await repo.GetPatientClinicalConditionByPatientId(pcc.Id);

            Assert.NotNull(dto);
            Assert.Equal(cc.Name, dto.ClinicalCondition);
        }

        [Fact]
        public async Task GetWithPatientConditionAndMedicines_Returns_Entity()
        {
            using var context = CreateContext("pcc_with_rel");
            var patient = new Patient { Name = "PCP2", Document = "d2", Phone = "0", Gender = "F", Cpf = "c2", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.Add(patient);
            var cc = new ClinicalCondition { Name = "Cond2", Type = "T2", Description = "D2" };
            context.ClinicalConditions.Add(cc);
            await context.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = cc.Id, DiagnosisDate = System.DateTime.Today, Observations = "obs2" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var repo = new PatientClinicalConditionRepository(context);
            var entity = await repo.GetWithPatientConditionAndMedicines(pcc.Id);

            Assert.NotNull(entity);
            Assert.Equal(patient.Id, entity.PatientId);
        }
    }
}
