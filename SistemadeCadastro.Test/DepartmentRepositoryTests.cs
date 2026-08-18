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
    public class DepartmentRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

        [Fact]
        public async Task GetDepartmentByFilter_Returns_Paged()
        {
            using var context = CreateContext("dept_filter");
            context.Departments.Add(new Department { Name = "DeptA", Description = "d" });
            context.Departments.Add(new Department { Name = "DeptB", Description = "d2" });
            await context.SaveChangesAsync();

            var repo = new DepartmentRepository(context);
            var filter = new DepartmentFilterDTO { Page = 1, Name = "DeptA" };
            var paged = await repo.GetDepartmentByFilter(filter);

            Assert.Single(paged.Departments);
            Assert.Equal("DeptA", paged.Departments.First().Name);
        }
    }
}
