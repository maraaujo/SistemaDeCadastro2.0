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

        public AvailablePermissionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AvailablePermission?> GetById(long id)
        {
            return await _context.AvailablePermissions.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
