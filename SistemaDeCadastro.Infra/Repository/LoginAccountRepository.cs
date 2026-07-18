
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

        public LoginAccountRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<LoginAccount?> GetById(long id)
        {
            return await _context.LoginAccounts.FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<LoginAccount?> GetByUserId(long userId)
        {
            return await _context.LoginAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
        }
        public async Task<LoginAccount?> GetByEmail(string email)
        {
            return await _context.LoginAccounts
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
