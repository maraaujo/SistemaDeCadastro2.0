using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAccessLogApp
    {
        Task<List<AccessLog>> GetAll();
        Task<AccessLog?> GetById(long id);
        Task<ApiResponse> Create(AccessLog entity);
        Task<ApiResponse> Update(AccessLog entity);
        Task<ApiResponse> Delete(long id);
    }
}
