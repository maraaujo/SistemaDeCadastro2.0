using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAvailablePermissionApp
    {
        Task<List<AvailablePermission>> GetAll();
        Task<AvailablePermission?> GetById(long id);
        Task<ApiResponse> Create(CreateAvailablePermissionDTO entity);
        Task<ApiResponse> Update(UpdateAvailablePermissionDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
