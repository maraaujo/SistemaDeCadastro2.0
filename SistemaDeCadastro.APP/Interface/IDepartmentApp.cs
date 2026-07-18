using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IDepartmentApp
    {
        Task<List<Department>> GetAll();
        Task<Department?> GetById(long id);
        Task<ApiResponse> Create(CreateDepartmentDTO entity);
        Task<ApiResponse> Update(UpdateDepartmentDTO  entity);
        Task<ApiResponse> Delete(long id);
    }
}
