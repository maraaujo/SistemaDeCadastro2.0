using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAccessLogRepository : IBaseRepository<AccessLog>
    {
        /// <summary>Busca um log de acesso pela chave primaria (null se nao existir ou for de outra instituicao).</summary>
        Task<AccessLog?> GetById(long id);
        /// <summary>Lista todos os logs de acesso gerados por um usuario (UserId da conta de acesso).</summary>
        Task<List<AccessLog>> GetByUserId(long userId);
        Task<List<AccessLog>> GetRecent(int max);
        /// <summary>Lista paginada dos logs de acesso com nome/e-mail do usuario, filtrando por usuario, e-mail, acao e dia.</summary>
        Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter);
    }
}
