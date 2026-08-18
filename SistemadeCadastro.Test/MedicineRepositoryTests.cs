using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Repository;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Filters;

namespace SistemadeCadastro.Test
{
    public class MedicineRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

        [Fact]
        public async Task GetMedicineById_Returns_List()
        {
            using var context = CreateContext("med_byid");
            var med = new Medicine { Name = "Med1", Description = "D", Dosage = "10mg", AdministrationRoute = "oral" };
            context.Medicines.Add(med);
            await context.SaveChangesAsync();

            var repo = new MedicineRepository(context);
            var list = await repo.GetMedicineById(med.Id);

            Assert.Single(list);
            Assert.Equal("Med1", list.First().Name);
        }

        [Fact]
        public async Task GetMedicineByFilter_Returns_Paged()
        {
            using var context = CreateContext("med_filter");
            context.Medicines.Add(new Medicine { Name = "FilterA", Description = "D", Dosage = "10", AdministrationRoute = "oral" });
            context.Medicines.Add(new Medicine { Name = "Other", Description = "D2", Dosage = "20", AdministrationRoute = "iv" });
            await context.SaveChangesAsync();

            var repo = new MedicineRepository(context);
            var filter = new MedicineFilterDTO { Page = 1, Name = "FilterA" };
            var paged = await repo.GetMedicineByFilter(filter);

            Assert.Single(paged.Medicines);
            Assert.Equal("FilterA", paged.Medicines.First().Name);
        }
    }
}
