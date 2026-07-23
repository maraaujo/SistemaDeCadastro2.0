using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IEmployeeApp
    {
        Task<List<Employee>> GetAll();
        Task<Employee?> GetById(long id);
        Task<ApiResponse> Create(CreateEmployeeDTO entity);
        Task<ApiResponse> Update(UpdateEmployeeDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedEmployeeDTO> GetEmployeeByFilter(EmployeeFilterDTO filter);
    }
}
