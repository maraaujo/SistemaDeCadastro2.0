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
        /// Busca a assinatura ativa mais recente da instituicao, carregando o plano. Aceita os status
        /// "Active", "Ativo" e "Ativa" em qualquer caixa (o Create grava "Ativo"; o painel admin aceita os tres).
        /// Devolve null se nao houver.
        /// </summary>
        public async Task<Subscription> GetActiveByInstitutionId(long institutionId)
        {
            var activeStatuses = new[] { "active", "ativo", "ativa" };

            return await _context.Set<Subscription>()
                .Include(s => s.Plan)
                .Where(s => s.InstitutionId == institutionId && activeStatuses.Contains(s.Status.ToLower()))
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }
    }
}
