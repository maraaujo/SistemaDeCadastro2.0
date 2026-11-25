using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientClinicalConditionRepository : IBaseRepository<PatientClinicalCondition>
    {
        /// <summary>Busca o vinculo acolhido x condicao clinica pelo id.</summary>
        Task<PatientClinicalCondition?> GetById(long id);
        /// <summary>Busca o vinculo acolhido x condicao clinica carregando acolhido, condicao e medicamentos programados.</summary>
        Task<PatientClinicalCondition?> GetWithPatientConditionAndMedicines(long id);
        /// <summary>Lista paginada das condicoes clinicas dos acolhidos com nomes de acolhido e condicao, aplicando os filtros.</summary>
        Task<PagedPatientClinicalConditionDTO> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter);
        /// <summary>Retorna todas as condicoes clinicas do acolhido (lista vazia se nao tiver nenhuma).</summary>
        Task<List<PatientClinicalConditionDTO>> GetPatientClinicalConditionByPatientId(long id);

       }
}
