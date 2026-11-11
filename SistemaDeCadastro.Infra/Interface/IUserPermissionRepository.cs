using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IUserPermissionRepository : IBaseRepository<UserPermission>
    {
        /// <summary>Busca uma permissao concedida a usuario pelo id.</summary>
        Task<UserPermission?> GetById(long id);
        /// <summary>Lista as permissoes concedidas a um usuario.</summary>
        Task<List<UserPermission>> GetByUserId(long userId);
    }
}
