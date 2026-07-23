using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IAccessLogApp
    {
        Task<List<AccessLog>> GetAll();
        Task<AccessLog?> GetById(long id);
        Task<ApiResponse> Create(CreateAccessLogDTO entity);
        Task<ApiResponse> Update(UpdateAccessLogDTO  entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter);
    }
}
