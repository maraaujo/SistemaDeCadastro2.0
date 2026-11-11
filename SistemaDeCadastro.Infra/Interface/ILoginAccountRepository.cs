using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface ILoginAccountRepository : IBaseRepository<LoginAccount>
    {
        /// <summary>Busca a conta de acesso pelo id (contas nao tem filtro por instituicao).</summary>
        Task<LoginAccount?> GetById(long id);
        /// <summary>Busca a conta de acesso pelo UserId (chave usada por logs, agendamentos e pagamentos).</summary>
        Task<LoginAccount?> GetByUserId(long userId);
        /// <summary>Busca a conta de acesso pelo e-mail exato (usado no login).</summary>
        Task<LoginAccount?> GetByEmail(string email);
        /// <summary>Calcula o proximo id livre para uma conta nova (maior entre Id e UserId, mais 1).</summary>
        Task<long> GetNextAccountId();
    }
}
