using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientApp
    {
        Task<List<Patient>> GetPatientById(long id);
        Task<ApiResponse> CreatePatient(PatientDTO patient);
        Task<ApiResponse> UpdatePatient(PatientDTO patient);
        Task<ApiResponse> DeletePatient(PatientDTO patient);
        Task GetPatientByAny(string patient);
    }
}
