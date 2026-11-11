using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IInstitutionApp
    {
        /// <summary>Cadastra uma instituicao e devolve-a em Data.</summary>
        Task<ApiResponse> Create(CreateInstitutionDTO entity);
        /// <summary>Atualiza os dados de uma instituicao existente e devolve-a em Data.</summary>
        Task<ApiResponse> Update(UpdateInstitutionDTO entity);
        /// <summary>Exclui a instituicao do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve em Data a instituicao do Id informado.</summary>
        Task<ApiResponse> GetById(long id);
        /// <summary>Devolve em Data todas as instituicoes.</summary>
        Task<ApiResponse> GetAll();
    }
}
