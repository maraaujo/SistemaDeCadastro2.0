using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICareServiceApp
    {
        Task<List<CareService>> GetAll();
        Task<CareService?> GetById(long id);
        Task<ApiResponse> Create(CareService entity);
        Task<ApiResponse> Update(CareService entity);
        Task<ApiResponse> Delete(long id);
    }
}
