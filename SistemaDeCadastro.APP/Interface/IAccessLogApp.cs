using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IAccessLogApp
    {
        Task<List<AccessLog>> GetAll();
        Task<AccessLog?> GetById(long id);
        Task<ApiResponse> Create(CreateAccessLogDTO entity);
        Task<ApiResponse> Update(UpdateAccessLogDTO  entity);
        Task<ApiResponse> Delete(long id);
    }
}
