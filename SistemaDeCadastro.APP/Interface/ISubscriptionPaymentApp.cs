using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.APP.Interface
{
    public interface ISubscriptionPaymentApp
    {
        /// <summary>Registra um pagamento de assinatura e devolve-o em Data.</summary>
        Task<ApiResponse> Create(CreateSubscriptionPaymentDTO entity);
        /// <summary>Atualiza os dados de um pagamento de assinatura existente.</summary>
        Task<ApiResponse> Update(UpdateSubscriptionPaymentDTO entity);
        /// <summary>Exclui o pagamento de assinatura do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve em Data o pagamento de assinatura do Id informado.</summary>
        Task<ApiResponse> GetById(long id);
        /// <summary>Devolve em Data todos os pagamentos de assinatura.</summary>
        Task<ApiResponse> GetAll();
    }
}
