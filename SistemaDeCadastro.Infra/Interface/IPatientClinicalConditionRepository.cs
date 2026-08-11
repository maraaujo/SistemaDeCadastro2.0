using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientClinicalConditionRepository : IBaseRepository<PatientClinicalCondition>
    {
        Task<PatientClinicalCondition?> GetById(long id);
        Task<PatientClinicalCondition?> GetWithPatientConditionAndMedicines(long id);
        Task<PagedPatientClinicalConditionDTO> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter);
        Task<PatientClinicalConditionDTO> GetPatientClinicalConditionByPatientId(long id);

       }
}
