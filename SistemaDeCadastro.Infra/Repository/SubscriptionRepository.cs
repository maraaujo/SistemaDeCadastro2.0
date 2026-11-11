using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Repository
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public SubscriptionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca a assinatura pelo id carregando o plano e a instituicao; devolve null se nao existir.</summary>
        public async Task<Subscription> GetByIdWithPlanAndInstitution(long id)
        {
            return await _context.Set<Subscription>()
                .Include(s => s.Plan)
                .Include(s => s.Institution)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Busca a primeira assinatura da instituicao com Status exatamente "Active", carregando o plano.
        /// Devolve null se nao houver. Nao considera variacoes como "ativo"/"active" em minusculas.
        /// </summary>
        public async Task<Subscription> GetActiveByInstitutionId(long institutionId)
        {
            return await _context.Set<Subscription>()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.InstitutionId == institutionId && s.Status == "Active");
        }
    }
}
