using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IUserPermissionApp
    {
        /// <summary>Devolve todas as permissoes concedidas a usuarios.</summary>
        Task<List<UserPermission>> GetAll();
        /// <summary>Devolve a permissao de usuario do Id informado, ou null se nao existir.</summary>
        Task<UserPermission?> GetById(long id);
        /// <summary>Concede uma permissao a um usuario.</summary>
        Task<ApiResponse> Create(CreateUserPermissionDTO entity);
        /// <summary>Atualiza uma permissao de usuario existente.</summary>
        Task<ApiResponse> Update(UpdateUserPermissionDTO entity);
        /// <summary>Exclui a permissao de usuario do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
    }
}
