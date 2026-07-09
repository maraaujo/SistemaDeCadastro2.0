using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientIllnessApp
    {
        Task<List<PatientIllness>> GetAll();
        Task<PatientIllness?> GetById(long id);
        Task<ApiResponse> Create(PatientIllness entity);
        Task<ApiResponse> Update(PatientIllness entity);
        Task<ApiResponse> Delete(long id);
    }
}
