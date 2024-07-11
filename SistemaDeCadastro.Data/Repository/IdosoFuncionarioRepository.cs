using Microsoft.Extensions.Logging;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Data.Interface;

namespace SistemaDeCadastro.Data.Repository
{
    public class IdosoFuncionarioRepository : BaseRepository<IdosoFuncionario>, IIdosoFuncionarioRepository
    {
        private readonly ILogger<FuncionarioRepository> log;
        public SistemaDeCadastroContext context;
        public IdosoFuncionarioRepository(SistemaDeCadastroContext _context, ILogger _log) : base(_context)
        {
            _context = context;
            _log = log;
        }
        
        public async Task GetFuncionariByIdoso(int id)
        {
             await FindBy(c => c.Id == id);
        }
    }
}
