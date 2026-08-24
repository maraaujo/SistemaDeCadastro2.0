using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientApp
    {
        Task<List<Patient>> GetPatientById(long id);
        Task<ApiResponse> CreatePatient(CreatePatientDTO patient);
        Task<ApiResponse> UpdatePatient(PatientDTO patient);
        Task<ApiResponse> DeletePatient(long id);
        Task GetPatientByAny(string patient);
        Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter);
        Task<DetailsPatientDTO?> DetailsPatient(long id);
        Task<List<Patient>> GetAllPatients();

    }
}
