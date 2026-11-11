using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAvailablePermissionApp
    {
        /// <summary>Devolve todas as permissoes disponiveis no sistema.</summary>
        Task<List<AvailablePermission>> GetAll();
        /// <summary>Devolve a permissao disponivel do Id informado, ou null se nao existir.</summary>
        Task<AvailablePermission?> GetById(long id);
        /// <summary>Cadastra uma nova permissao disponivel (nome, descricao, modulo, ativa).</summary>
        Task<ApiResponse> Create(CreateAvailablePermissionDTO entity);
        /// <summary>Atualiza uma permissao disponivel existente.</summary>
        Task<ApiResponse> Update(UpdateAvailablePermissionDTO entity);
        /// <summary>Exclui a permissao disponivel do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
    }
}
