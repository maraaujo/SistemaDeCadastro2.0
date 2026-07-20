using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPaymentApp
    {
        Task<List<Payment>> GetAll();
        Task<Payment?> GetById(long id);
        Task<ApiResponse> Create(CreatePaymentDTO entity);
        Task<ApiResponse> Update(UpdatePaymentDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
