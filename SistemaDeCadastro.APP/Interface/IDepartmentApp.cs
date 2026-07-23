using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IDepartmentApp
    {
        Task<List<Department>> GetAll();
        Task<Department?> GetById(long id);
        Task<ApiResponse> Create(CreateDepartmentDTO entity);
        Task<ApiResponse> Update(UpdateDepartmentDTO  entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter);
      
    }
}
