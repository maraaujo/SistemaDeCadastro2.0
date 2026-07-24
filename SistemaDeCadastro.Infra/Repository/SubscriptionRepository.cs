using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Repository
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public SubscriptionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Subscription> GetByIdWithPlanAndInstitution(long id)
        {
            return await _context.Set<Subscription>()
                .Include(s => s.Plan)
                .Include(s => s.Institution)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subscription> GetActiveByInstitutionId(long institutionId)
        {
            return await _context.Set<Subscription>()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.InstitutionId == institutionId && s.Status == "Active");
        }
    }
}
