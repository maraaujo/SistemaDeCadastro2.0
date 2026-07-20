using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.APP.Interface
{
    public interface ISubscriptionPaymentApp
    {
        Task<ApiResponse> Create(CreateSubscriptionPaymentDTO entity);
        Task<ApiResponse> Update(UpdateSubscriptionPaymentDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<ApiResponse> GetById(long id);
        Task<ApiResponse> GetAll();
    }
}
