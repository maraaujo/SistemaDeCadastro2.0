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
    public class EmployeeRepositoryTests
    {
        private SistemaDeCadastroContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<SistemaDeCadastroContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SistemaDeCadastroContext(options, new TestCurrentUserService());
        }

        [Fact]
        public async Task GetByDepartmentId_Returns_List()
        {
            using var context = CreateContext("emp_by_dept");
            var dept = new Department { Name = "D1", Description = "desc" };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();

            var emp = new Employee { Name = "Emp1", Cpf = "111", Position = "pos", Phone = "0", Email = "e@e.com", AdmissionDate = System.DateTime.Now, DepartmentId = dept.Id };
            context.Employees.Add(emp);
            await context.SaveChangesAsync();

            var repo = new EmployeeRepository(context);
            var list = await repo.GetByDepartmentId(dept.Id);

            Assert.Single(list);
            Assert.Equal("Emp1", list.First().Name);
        }

        [Fact]
        public async Task GetEmployeeByFilter_Returns_Paged()
        {
            using var context = CreateContext("emp_filter");
            var dept = new Department { Name = "D2", Description = "desc2" };
            context.Departments.Add(dept);
            context.Employees.Add(new Employee { Name = "FilterEmp", Cpf = "333", Position = "p", Phone = "0", Email = "a@a.com", AdmissionDate = System.DateTime.Now, DepartmentId = dept.Id });
            context.Employees.Add(new Employee { Name = "OtherEmp", Cpf = "444", Position = "p2", Phone = "0", Email = "b@b.com", AdmissionDate = System.DateTime.Now, DepartmentId = dept.Id });
            await context.SaveChangesAsync();

            var repo = new EmployeeRepository(context);
            var filter = new EmployeeFilterDTO { Page = 1, Name = "FilterEmp" };
            var paged = await repo.GetEmployeeByFilter(filter);

            Assert.Single(paged.Employees);
            Assert.Equal("FilterEmp", paged.Employees.First().Name);
        }
    }
}
