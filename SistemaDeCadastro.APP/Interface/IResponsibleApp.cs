using Microsoft.AspNetCore.Identity;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IResponsibleApp
    {
        /// <summary>Devolve todos os responsaveis (familiares/contatos) dos acolhidos.</summary>
        Task<List<Responsible>> GetAll();
        /// <summary>Devolve o responsavel do Id informado, ou null se nao existir.</summary>
        Task<Responsible?> GetById(long id);
        /// <summary>Cadastra um responsavel para um acolhido da instituicao.</summary>
        Task<ApiResponse> Create(CreateResponsibleDTO entity);
        /// <summary>Atualiza os dados de um responsavel existente.</summary>
        Task<ApiResponse> Update(UpdateResponsibleDTO entity);
        /// <summary>Exclui o responsavel do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
    }
}
