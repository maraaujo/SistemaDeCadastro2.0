using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface ILoginAccountRepository : IBaseRepository<LoginAccount>
    {
        Task<LoginAccount?> GetById(long id);
        Task<LoginAccount?> GetByUserId(long userId);
        Task<LoginAccount?> GetByEmail(string email);
    }
}
