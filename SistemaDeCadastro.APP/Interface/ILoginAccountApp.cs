using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ILoginAccountApp
    {
        Task<List<LoginAccount>> GetAll();
        Task<LoginAccount?> GetById(long id);
        Task<ApiResponse> Create(CreateLoginAccountDTO entity);
        Task<ApiResponse> Update(UpdateLoginAccountDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
