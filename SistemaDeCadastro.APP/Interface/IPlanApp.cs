using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IPlanApp
    {
        /// <summary>Cadastra um plano de assinatura e devolve-o em Data.</summary>
        Task<ApiResponse> Create(CreatePlanDTO entity);
        /// <summary>Atualiza os dados de um plano existente e devolve-o em Data.</summary>
        Task<ApiResponse> Update(UpdatePlanDTO entity);
        /// <summary>Exclui o plano do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve em Data o plano do Id informado.</summary>
        Task<ApiResponse> GetById(long id);
        /// <summary>Devolve em Data todos os planos.</summary>
        Task<ApiResponse> GetAll();
    }
}
