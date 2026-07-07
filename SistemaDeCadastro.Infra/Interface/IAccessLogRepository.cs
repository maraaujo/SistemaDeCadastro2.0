using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAccessLogRepository : IBaseRepository<AccessLog>
    {
        Task<AccessLog?> GetById(long id);
        Task<List<AccessLog>> GetByUserId(long userId);
    }
}
