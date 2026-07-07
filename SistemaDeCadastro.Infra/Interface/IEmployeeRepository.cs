using SistemaDeCadastro.Domain.Models.Stage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee?> GetById(long id);
        Task<List<Employee>> GetByDepartmentId(long departmentId);
    }
}
