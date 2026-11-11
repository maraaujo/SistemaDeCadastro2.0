using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPaymentApp
    {
        /// <summary>Devolve todos os pagamentos da instituicao.</summary>
        Task<List<Payment>> GetAll();
        /// <summary>Devolve o pagamento do Id informado, ou null se nao existir.</summary>
        Task<Payment?> GetById(long id);
        /// <summary>Monta um pagamento a partir do DTO e devolve-o em Data.</summary>
        Task<ApiResponse> Create(CreatePaymentDTO entity);
        /// <summary>Atualiza os dados de um pagamento existente.</summary>
        Task<ApiResponse> Update(UpdatePaymentDTO entity);
        /// <summary>Exclui o pagamento do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
    }
}
