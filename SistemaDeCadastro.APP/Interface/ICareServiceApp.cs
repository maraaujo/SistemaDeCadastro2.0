using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICareServiceApp
    {
        Task<List<CareService>> GetAll();
        Task<CareService?> GetById(long id);
        Task<ApiResponse> Create(CreateCareServiceDTO entity);
        Task<ApiResponse> Update(UpdateCareServiceDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedCareServiceDTO> GetCareServiceByFilter(CareServiceFilterDTO filter);
    }
}
