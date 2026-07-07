using SistemaDeCadastro.Domain.Models.Stage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department?> GetById(long id);
    }
}
