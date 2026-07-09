using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IUserPermissionApp
    {
        Task<List<UserPermission>> GetAll();
        Task<UserPermission?> GetById(long id);
        Task<ApiResponse> Create(UserPermission entity);
        Task<ApiResponse> Update(UserPermission entity);
        Task<ApiResponse> Delete(long id);
    }
}
