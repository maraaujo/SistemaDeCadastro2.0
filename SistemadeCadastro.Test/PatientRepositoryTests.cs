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
    public class PatientRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

        [Fact]
        public async Task DetailsPatient_Returns_Details()
        {
            using var context = CreateContext("patient_details");

            var blood = new BloodType { Name = "A+" };
            context.BloodTypes.Add(blood);

            var patient = new Patient
            {
                Name = "Paciente Test",
                Document = "DOC123",
                Phone = "99999999",
                Gender = "M",
                Cpf = "00011122233",
                Observations = "obs",
                BirthDate = System.DateTime.Today.AddYears(-30),
                CreatedAt = System.DateTime.Now,
                BloodType = blood
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var responsible = new Responsible { PatientId = patient.Id, Name = "Resp", Phone = "8888", Relationship = "Pai", Address = "Addr" };
            context.Responsibles.Add(responsible);

            var cc = new ClinicalCondition { Name = "Cond", Type = "T", Description = "D" };
            context.ClinicalConditions.Add(cc);
            await context.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient.Id, ClinicalConditionId = cc.Id, DiagnosisDate = System.DateTime.Today, Observations = "pcc obs" };
            context.PatientClinicalConditions.Add(pcc);

            var med = new Medicine { Name = "MedX", Description = "D", Dosage = "10mg", AdministrationRoute = "oral" };
            context.Medicines.Add(med);
            await context.SaveChangesAsync();

            var mpcc = new MedicinePatientClinicalCondition
            {
                PatientClinicalConditionId = pcc.Id,
                MedicineId = med.Id,
                ResponsibleEmployeeId = null,
                Frequency = "daily",
                AdministrationTime = System.TimeSpan.FromHours(9),
                PrescribedDosage = "10mg",
                StartDate = System.DateTime.Today,
            };
            context.MedicinePatientClinicalConditions.Add(mpcc);

            var appt = new Appointment { PatientId = patient.Id, UserId = 1, AppointmentType = "T", DateTime = System.DateTime.Now, Responsible = "R", Status = "S", Observations = "O" };
            context.Appointments.Add(appt);

            await context.SaveChangesAsync();

            var repo = new PatientRepository(context);
            var details = await repo.DetailsPatient(patient.Id);

            Assert.NotNull(details);
            Assert.Equal(patient.Name, details.Patient.Name);
            Assert.NotEmpty(details.Responsibles);
            Assert.NotEmpty(details.ClinicalConditions);
            Assert.NotEmpty(details.Medicines);
        }

        [Fact]
        public async Task FilterPatient_ByNameAndClinicalCondition_Returns_Paged()
        {
            using var context = CreateContext("patient_filter");

            var patient1 = new Patient { Name = "Alice", Document = "d1", Phone = "1", Gender = "F", Cpf = "c1", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            var patient2 = new Patient { Name = "Bob", Document = "d2", Phone = "2", Gender = "M", Cpf = "c2", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now };
            context.Patients.AddRange(patient1, patient2);

            var cc = new ClinicalCondition { Name = "CondX", Type = "T", Description = "D" };
            context.ClinicalConditions.Add(cc);
            await context.SaveChangesAsync();

            var pcc = new PatientClinicalCondition { PatientId = patient1.Id, ClinicalConditionId = cc.Id, DiagnosisDate = System.DateTime.Today, Observations = "obs" };
            context.PatientClinicalConditions.Add(pcc);
            await context.SaveChangesAsync();

            var repo = new PatientRepository(context);
            var filter = new SistemaDeCadastro.Domain.DataTransferObject.PatientFilterDTO { Page = 1, Name = "Alice", ClinicalConditionIds = new System.Collections.Generic.List<long> { cc.Id } };
            var paged = await repo.FilterPatient(filter);

            Assert.Single(paged.Patients);
            Assert.Equal("Alice", paged.Patients.First().Name);
        }

        [Fact]
        public async Task GetByIdWithRelations_Returns_Patient_With_Relations()
        {
            using var context = CreateContext("patient_with_rel");
            var blood = new BloodType { Name = "B+" };
            context.BloodTypes.Add(blood);

            var patient = new Patient { Name = "WithRel", Document = "d", Phone = "p", Gender = "F", Cpf = "c", Observations = "o", BirthDate = System.DateTime.Today, CreatedAt = System.DateTime.Now, BloodType = blood };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var responsible = new Responsible { PatientId = patient.Id, Name = "R1", Phone = "p1", Relationship = "M", Address = "A" };
            context.Responsibles.Add(responsible);
            await context.SaveChangesAsync();

            var repo = new PatientRepository(context);
            var entity = await repo.GetByIdWithRelations(patient.Id);

            Assert.NotNull(entity);
            Assert.Equal(patient.Name, entity.Name);
            Assert.NotEmpty(entity.Responsibles);
            Assert.NotNull(entity.BloodType);
        }

        
    }
}
