using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Repository;

namespace SistemadeCadastro.Test
{
    public class PatientAppTest
    {
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
    }
}