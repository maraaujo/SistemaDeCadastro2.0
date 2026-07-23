using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAccessLogRepository : IBaseRepository<AccessLog>
    {
        Task<AccessLog?> GetById(long id);
        Task<List<AccessLog>> GetByUserId(long userId);
        Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter);
    }
}
