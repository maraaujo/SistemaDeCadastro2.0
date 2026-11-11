using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class AvailablePermissionRepository : BaseRepository<AvailablePermission>, IAvailablePermissionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public AvailablePermissionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca uma permissao do catalogo de permissoes disponiveis pelo id (null se nao existir).</summary>
        public async Task<AvailablePermission?> GetById(long id)
        {
            return await _context.AvailablePermissions.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
