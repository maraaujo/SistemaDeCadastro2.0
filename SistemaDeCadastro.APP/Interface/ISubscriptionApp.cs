using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.APP.Interface
{
    public interface ISubscriptionApp
    {
        /// <summary>Cria uma assinatura ativa de uma instituicao em um plano.</summary>
        Task<ApiResponse> Create(CreateSubscriptionDTO entity);
        /// <summary>Atualiza os dados de uma assinatura existente.</summary>
        Task<ApiResponse> Update(UpdateSubscriptionDTO entity);
        /// <summary>Exclui a assinatura do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve em Data a assinatura do Id informado.</summary>
        Task<ApiResponse> GetById(long id);
        /// <summary>Devolve em Data todas as assinaturas.</summary>
        Task<ApiResponse> GetAll();
        /// <summary>Devolve em Data a assinatura ativa da instituicao informada.</summary>
        Task<ApiResponse> GetActiveByInstitutionId(long institutionId);
    }
}
