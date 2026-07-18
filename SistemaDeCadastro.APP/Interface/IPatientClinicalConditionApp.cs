using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientClinicalConditionApp
    {
        Task<List<PatientClinicalCondition>> GetAll();
        Task<PatientClinicalCondition?> GetById(long id);
        Task<ApiResponse> Create(CreatePatientClinicalConditionDTO entity);
        Task<ApiResponse> Update(PatientClinicalCondition entity);
        Task<ApiResponse> Delete(long id);
    }
}
