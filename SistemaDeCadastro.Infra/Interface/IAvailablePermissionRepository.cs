using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAvailablePermissionRepository : IBaseRepository<AvailablePermission>
    {
        /// <summary>Busca uma permissao disponivel (catalogo de permissoes) pelo id.</summary>
        Task<AvailablePermission?> GetById(long id);
    }
}
