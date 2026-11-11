using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ICareServiceApp
    {
        /// <summary>Devolve todos os atendimentos (servicos de cuidado) cadastrados.</summary>
        Task<List<CareService>> GetAll();
        /// <summary>Devolve o atendimento do Id informado, ou null se nao existir.</summary>
        Task<CareService?> GetById(long id);
        /// <summary>Cadastra um novo atendimento.</summary>
        Task<ApiResponse> Create(CreateCareServiceDTO entity);
        /// <summary>Atualiza um atendimento existente.</summary>
        Task<ApiResponse> Update(UpdateCareServiceDTO entity);
        /// <summary>Exclui o atendimento do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de atendimentos conforme o filtro.</summary>
        Task<PagedCareServiceDTO> GetCareServiceByFilter(CareServiceFilterDTO filter);
    }
}
