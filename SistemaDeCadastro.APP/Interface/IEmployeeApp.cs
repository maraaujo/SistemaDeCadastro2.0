using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IEmployeeApp
    {
        Task<List<Employee>> GetAll();
        Task<Employee?> GetById(long id);
        Task<ApiResponse> Create(CreatePatientEmployeeDTO entity);
        Task<ApiResponse> Update(Employee entity);
        Task<ApiResponse> Delete(long id);
    }
}
