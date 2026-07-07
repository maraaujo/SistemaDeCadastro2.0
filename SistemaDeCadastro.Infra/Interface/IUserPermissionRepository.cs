using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IUserPermissionRepository : IBaseRepository<UserPermission>
    {
        Task<UserPermission?> GetById(long id);
        Task<List<UserPermission>> GetByUserId(long userId);
    }
}
