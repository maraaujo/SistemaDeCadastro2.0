using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public UserPermissionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca uma permissao concedida a usuario pelo id (tabela sem filtro por instituicao).</summary>
        public async Task<UserPermission?> GetById(long id)
        {
            return await _context.UserPermissions.FirstOrDefaultAsync(up => up.Id == id);
        }

        /// <summary>Lista todas as permissoes concedidas ao usuario informado.</summary>
        public async Task<List<UserPermission>> GetByUserId(long userId)
        {
            return await _context.UserPermissions.Where(up => up.UserId == userId).ToListAsync();
        }
    }
}
