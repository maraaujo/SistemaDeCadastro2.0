using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department?> GetById(long id);
        Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter);
    }
}
