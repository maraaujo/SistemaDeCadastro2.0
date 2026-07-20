using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IEmployeeApp
    {
        Task<List<Employee>> GetAll();
        Task<Employee?> GetById(long id);
        Task<ApiResponse> Create(CreateEmployeeDTO entity);
        Task<ApiResponse> Update(UpdateEmployeeDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
