using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    public class PatientAppTest
    {
        private readonly PatientApp _patientApp;
        [Fact]
        public async Task GetPatientById()
        {
            //Arrange
            using var context = TestDbContextFactory.CreateDbContext();

            var existingPatientId = 1;
            var patientApp = new PatientApp(new PatientRepository(context), null);

            //Act 
            var result = await patientApp.GetPatientById(existingPatientId);

            //Assert 

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Felipe Campelo", result.First().Name);

        }
        [Fact]
        public async Task CreatePatient()
        {
            //arrange 
            using var context = TestDbContextFactory.CreateDbContext();
            var createPatient = new CreatepatientDTO
            {
                Id = 0,
                Name = "João",
                BooldType = 1,
                Phone = "987654321",
                Responsible = "João Junior",
                MedicinePatientIllnesses = new List<MedicinePatientIllnessDTO>
                {
                    new MedicinePatientIllnessDTO { IdIllness = 1, 
                        IdMedicine = 1,
                        Dosage = "10mg", 
                        Time = 8 }
                }

            }; 
            //Act 
            var result = await _patientApp.CreatePatient(createPatient);

            //Assert
            Assert.True(result.Success);
            var patient = context.Patients.FirstOrDefault();
            Assert.NotNull(patient);
            Assert.Equal("João", patient.Name);
        }

        [Fact]
        public async Task UpdatePatient()
        {
            //Arrange
            using var context = TestDbContextFactory.CreateDbContext();
            var existingPatient = new Patient
            {
                Name = "João",
                Document = "123456",
                Responsible = "Old Responsible",
                Phone = "123456789",
                IdBloodType = 1
            }; 
            context.Patients.Add(existingPatient);
            await context.SaveChangesAsync();

            var updatePatient = new PatientDTO
            {
                Id = existingPatient.Id,
                Name = "João Neto",
                Document = "654321",
                Responsible = "New Responsible",
                Phone = "987654321",
                IdBloodType = 2
            };

            //Act 
            var response = await _patientApp.UpdatePatient(updatePatient);

            //Assert

            Assert.True(response.Success);
            var patient = context.Patients.FirstOrDefault( p => p.Id == existingPatient.Id );   
            Assert.NotNull(patient);
            Assert.Equal("João Neto", patient.Name );
            Assert.Equal("654321", patient.Document);
        }

        [Fact]
        public async Task DeletePatient()
        {
            //Arrange
            using var context = TestDbContextFactory.CreateDbContext();
            var patient = new Patient
            {
                Name = "John Doe",
                Document = "123456",
                Responsible = "Jane Doe",
                Phone = "123456789",
                IdBloodType = 1
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            //Act
            var responte = await _patientApp.DeletePatient(patient.Id);

            //Asset 
            Assert.True(responte.Success);
            Assert.Null(context.Patients.FirstOrDefault(p => p.Id == patient.Id));
        }
        //[Fact]
        //public async Task CreateNewDosageInterval_ShouldAddToHistoricTable()
        //{
        //    // Arrange
        //    using var context = TestDbContextFactory.CreateDbContext();
        //    var medicinePatientIllness = new MedicinePatientIllness
        //    {
        //        IdIllness = 1,
        //        IdMedicine = 1,
        //        Dosage = "1x ao dia",
        //        Time = 8
        //    };
        //    context.MedicinePatientIllnesses.Add(medicinePatientIllness);
        //    await context.SaveChangesAsync();

        //    var historicDTO = new MedicinePatientIllnessHistoricDTO
        //    {
        //        IdMedicinePatientIllness = medicinePatientIllness.Id,
        //        LastTime = DateTime.Now
        //    };

        //    // Act
        //    var response = await _patientApp.CreateNewDosageInterval(historicDTO);

        //    // Assert
        //    Assert.True(response.Success);
        //    var historic = context.MedicinePatientIllnessHistorics.FirstOrDefault();
        //    Assert.NotNull(historic);
        //    Assert.Equal(medicinePatientIllness.Id, historic.IdMedicinePatientIllness);
        //}
    }
}