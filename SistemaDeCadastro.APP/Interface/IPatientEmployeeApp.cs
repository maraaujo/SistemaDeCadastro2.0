using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientEmployeeApp
    {
        Task<List<PatientEmployee>> GetAll();
        Task<PatientEmployee?> GetById(long id);
        Task<ApiResponse> Create(CreatePatientEmployeeDTO entity);
        Task<ApiResponse> Update(UpdatePatientEmployeeDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedPatientEmployeeDTO> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter);
    }
}
