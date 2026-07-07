using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class ClinicalConditionRepository : BaseRepository<ClinicalCondition>, IClinicalConditionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public ClinicalConditionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ClinicalCondition?> GetById(long id)
        {
            return await _context.ClinicalConditions.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
