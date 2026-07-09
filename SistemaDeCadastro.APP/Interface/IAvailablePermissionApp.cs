using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAvailablePermissionApp
    {
        Task<List<AvailablePermission>> GetAll();
        Task<AvailablePermission?> GetById(long id);
        Task<ApiResponse> Create(AvailablePermission entity);
        Task<ApiResponse> Update(AvailablePermission entity);
        Task<ApiResponse> Delete(long id);
    }
}
