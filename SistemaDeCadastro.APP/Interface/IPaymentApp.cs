using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPaymentApp
    {
        Task<List<Payment>> GetAll();
        Task<Payment?> GetById(long id);
        Task<ApiResponse> Create(Payment entity);
        Task<ApiResponse> Update(Payment entity);
        Task<ApiResponse> Delete(long id);
    }
}
