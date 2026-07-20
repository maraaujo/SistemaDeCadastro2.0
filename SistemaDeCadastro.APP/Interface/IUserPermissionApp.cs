using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IUserPermissionApp
    {
        Task<List<UserPermission>> GetAll();
        Task<UserPermission?> GetById(long id);
        Task<ApiResponse> Create(CreateUserPermissionDTO entity);
        Task<ApiResponse> Update(UpdateUserPermissionDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
