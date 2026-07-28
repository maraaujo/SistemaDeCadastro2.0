using SistemaDeCadastro.Domain.DataTransferObject;  
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Pageds;  
namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientIllnessApp
    {
        Task<List<PatientIllness>> GetAll();
        Task<PatientIllness?> GetById(long id);
        Task<ApiResponse> Create(CreatePatientIllnessDTO entity);
        Task<ApiResponse> Update(UpdatePatientIllnessDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedPatientIllnessDTO> GetPatientIllnessByFilter(PatientIllnessFilterDTO filter);
    }
}
