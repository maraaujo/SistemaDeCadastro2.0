using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee?> GetById(long id);
        Task<List<Employee>> GetByDepartmentId(long departmentId);
        Task<PagedEmployeeDTO> GetEmployeeByFilter(EmployeeFilterDTO filter);
    }
}
