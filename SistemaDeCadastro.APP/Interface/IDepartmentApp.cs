using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IDepartmentApp
    {
        Task<List<Department>> GetAll();
        Task<Department?> GetById(long id);
        Task<ApiResponse> Create(Department entity);
        Task<ApiResponse> Update(Department entity);
        Task<ApiResponse> Delete(long id);
    }
}
