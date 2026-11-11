using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        /// <summary>Busca a assinatura pelo id carregando o plano e a instituicao (null se nao existir).</summary>
        Task<Subscription> GetByIdWithPlanAndInstitution(long id);
        /// <summary>Busca a assinatura com status "Active" da instituicao, carregando o plano (null se nao houver).</summary>
        Task<Subscription> GetActiveByInstitutionId(long institutionId);
    }
}
