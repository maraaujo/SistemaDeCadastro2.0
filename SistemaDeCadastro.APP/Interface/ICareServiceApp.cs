using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICareServiceApp
    {
        Task<List<CareService>> GetAll();
        Task<CareService?> GetById(long id);
        Task<ApiResponse> Create(CreateCareServiceDTO entity);
        Task<ApiResponse> Update(UpdateCareServiceDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
