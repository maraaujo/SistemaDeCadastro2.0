using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientEmployeeApp
    {
        Task<List<PatientEmployee>> GetAll();
        Task<PatientEmployee?> GetById(long id);
        Task<ApiResponse> Create(PatientEmployee entity);
        Task<ApiResponse> Update(PatientEmployee entity);
        Task<ApiResponse> Delete(long id);
    }
}
