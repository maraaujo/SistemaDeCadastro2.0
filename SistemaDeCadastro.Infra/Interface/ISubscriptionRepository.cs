using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        Task<Subscription> GetByIdWithPlanAndInstitution(long id);
        Task<Subscription> GetActiveByInstitutionId(long institutionId);
    }
}
