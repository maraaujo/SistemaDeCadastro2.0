using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;  
namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientIllnessApp
    {
        Task<List<PatientIllness>> GetAll();
        Task<PatientIllness?> GetById(long id);
        Task<ApiResponse> Create(CreatePatientIllnessDTO entity);
        Task<ApiResponse> Update(UpdatePatientIllnessDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
