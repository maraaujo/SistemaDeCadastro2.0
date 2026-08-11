using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientClinicalConditionApp
    {
        Task<List<PatientClinicalCondition>> GetAll();
        Task<PatientClinicalCondition?> GetById(long id);
        Task<ApiResponse> Create(CreatePatientClinicalConditionDTO entity);
        Task<ApiResponse> Update(PatientClinicalCondition entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedPatientClinicalConditionDTO> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter);
        Task<PatientClinicalConditionDTO> GetPatientClinicalConditionByPatientId(long id);
    }
}
