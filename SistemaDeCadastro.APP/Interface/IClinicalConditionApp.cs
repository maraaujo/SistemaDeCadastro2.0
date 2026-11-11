using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IClinicalConditionApp
    {
        /// <summary>Devolve todas as condicoes clinicas do catalogo.</summary>
        Task<List<ClinicalCondition>> GetAll();
        /// <summary>Devolve a condicao clinica do Id informado, ou null se nao existir.</summary>
        Task<ClinicalCondition?> GetById(long id);
        /// <summary>Cadastra uma condicao clinica (nome, descricao, tipo).</summary>
        Task<ApiResponse> Create(CreateClinicalConditionDTO entity);
        /// <summary>Atualiza uma condicao clinica existente.</summary>
        Task<ApiResponse> Update(UpdateClinicalConditionDTO entity);
        /// <summary>Exclui a condicao clinica do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de condicoes clinicas conforme o filtro.</summary>
        Task<PagedClinicalConditionDTO> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter);
    }
}
