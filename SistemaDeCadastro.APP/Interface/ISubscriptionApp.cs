using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.APP.Interface
{
    public interface ISubscriptionApp
    {
        Task<ApiResponse> Create(CreateSubscriptionDTO entity);
        Task<ApiResponse> Update(UpdateSubscriptionDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<ApiResponse> GetById(long id);
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetActiveByInstitutionId(long institutionId);
    }
}
