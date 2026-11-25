using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientClinicalConditionApp
    {
        /// <summary>Devolve todos os vinculos acolhido + condicao clinica da instituicao.</summary>
        Task<List<PatientClinicalCondition>> GetAll();
        /// <summary>Devolve o vinculo acolhido + condicao clinica do Id informado, ou null se nao existir.</summary>
        Task<PatientClinicalCondition?> GetById(long id);
        /// <summary>Vincula uma condicao clinica a um acolhido da instituicao.</summary>
        Task<ApiResponse> Create(CreatePatientClinicalConditionDTO entity);
        /// <summary>Atualiza um vinculo acolhido + condicao clinica existente.</summary>
        Task<ApiResponse> Update(PatientClinicalCondition entity);
        /// <summary>Exclui o vinculo e, antes, seus medicamentos programados e administracoes.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de condicoes clinicas dos acolhidos conforme o filtro.</summary>
        Task<PagedPatientClinicalConditionDTO> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter);
        /// <summary>Devolve as condicoes clinicas do acolhido informado.</summary>
        Task<List<PatientClinicalConditionDTO>> GetPatientClinicalConditionByPatientId(long id);
    }
}
