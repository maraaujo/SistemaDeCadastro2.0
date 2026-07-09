using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ILoginAccountApp
    {
        Task<List<LoginAccount>> GetAll();
        Task<LoginAccount?> GetById(long id);
        Task<ApiResponse> Create(LoginAccount entity);
        Task<ApiResponse> Update(LoginAccount entity);
        Task<ApiResponse> Delete(long id);
    }
}
