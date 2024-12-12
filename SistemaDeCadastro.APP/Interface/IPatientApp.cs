using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientApp
    {
        Task<List<Patient>> GetPatientById(long id);
        Task<ApiResponse> CreatePatient(CreatepatientDTO patient);
        Task<ApiResponse> UpdatePatient(PatientDTO patient);
        Task<ApiResponse> DeletePatient(PatientDTO patient);
        Task GetPatientByAny(string patient);
        Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter);
        Task<List<DetailsPatientDTO>> DetailsPatient();


    }
}
