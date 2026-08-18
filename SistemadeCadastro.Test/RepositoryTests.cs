using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.APP.Interface;

namespace SistemadeCadastro.Test
{
    public class RepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            // TestCurrentUserService with null InstitutionId is ok for most repository tests
            var currentUser = new TestCurrentUserService();
            return new SistemaDeCadastroContext(options, currentUser);
        }

        [Fact]
        public async Task BaseRepository_Create_And_GetAll_Works()
        {
            using var context = CreateContext("BaseRepo_Create_GetAll");
            var repo = new DepartmentRepository(context);

            var dept = new Department { Name = "Teste", Description = "Desc" };
            await repo.Create(dept);

            var all = await repo.GetAll();
            Assert.Single(all);
            Assert.Equal("Teste", all.First().Name);
        }

        [Fact]
        public async Task BaseRepository_FindBy_And_Any_Works()
        {
            using var context = CreateContext("BaseRepo_FindBy_Any");
            var repo = new DepartmentRepository(context);

            var d1 = new Department { Name = "A", Description = "d" };
            var d2 = new Department { Name = "B", Description = "d" };
            await repo.Create(d1);
            await repo.Create(d2);

            var found = await repo.FindBy(x => x.Name == "A");
            Assert.Single(found);
            Assert.True(await repo.Any(x => x.Name == "B"));
        }

        [Fact]
        public async Task BaseRepository_Update_And_Delete_Works()
        {
            using var context = CreateContext("BaseRepo_Update_Delete");
            var repo = new DepartmentRepository(context);

            var d = new Department { Name = "ToUpdate", Description = "d" };
            await repo.Create(d);

            var created = (await repo.GetAll()).First();
            created.Description = "Updated";
            await repo.Update(created);

            var updated = (await repo.GetAll()).First();
            Assert.Equal("Updated", updated.Description);

            await repo.Delete(updated);
            var afterDelete = await repo.GetAll();
            Assert.Empty(afterDelete);
        }

        [Fact]
        public async Task PatientRepository_DetailsPatient_Returns_Details()
        {
            using var context = CreateContext("Patient_Details");
            // preparar dados relacionados
            var patient = new Patient
            {
                Name = "Paciente X",
                Document = "111",
                Phone = "00000000",
                Gender = "M",
                Cpf = "00000000000",
                Observations = "obs",
                BirthDate = DateTime.Today.AddYears(-30),
                CreatedAt = DateTime.Now
            };
            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            var repo = new PatientRepository(context);
            var details = await repo.DetailsPatient(patient.Id);

            Assert.NotNull(details);
            Assert.Equal("Paciente X", details.Patient.Name);
        }
    }

    // Reutiliza TestCurrentUserService definido em TestDbContextFactory
}
