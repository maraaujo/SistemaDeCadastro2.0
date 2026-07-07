using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAvailablePermissionRepository : IBaseRepository<AvailablePermission>
    {
        Task<AvailablePermission?> GetById(long id);
    }
}
