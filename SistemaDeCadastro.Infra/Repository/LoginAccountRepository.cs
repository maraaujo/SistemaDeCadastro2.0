
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class LoginAccountRepository : BaseRepository<LoginAccount>, ILoginAccountRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public LoginAccountRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca a conta de acesso pelo id. Contas nao tem filtro por instituicao no contexto.</summary>
        public async Task<LoginAccount?> GetById(long id)
        {
            return await _context.LoginAccounts.FirstOrDefaultAsync(l => l.Id == id);
        }

        /// <summary>Busca a conta de acesso pelo UserId (a chave que logs, agendamentos e pagamentos referenciam).</summary>
        public async Task<LoginAccount?> GetByUserId(long userId)
        {
            return await _context.LoginAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
        }
        /// <summary>Busca a conta de acesso pelo e-mail (comparacao exata, de todas as instituicoes).</summary>
        public async Task<LoginAccount?> GetByEmail(string email)
        {
            return await _context.LoginAccounts
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        // Proximo id livre para uma conta nova. Id e UserId sao gravados com o mesmo
        // valor ja no insert: o UserId e chave alternativa (log_acesso, agendamentos e
        // pagamentos apontam para ele) e o EF nao deixa altera-lo depois de gravado.
        public async Task<long> GetNextAccountId()
        {
            var maxId = await _context.LoginAccounts.MaxAsync(l => (long?)l.Id) ?? 0;
            var maxUserId = await _context.LoginAccounts.MaxAsync(l => (long?)l.UserId) ?? 0;

            return System.Math.Max(maxId, maxUserId) + 1;
        }
    }
}
