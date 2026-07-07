using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public DepartmentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Department?> GetById(long id)
        {
            return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
