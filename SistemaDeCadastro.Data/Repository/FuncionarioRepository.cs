using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Data.Repository
{
    public class FuncionarioRepository : BaseRepository<Funcionario>, IFuncionarioRepository
    {
        private readonly ILogger<FuncionarioRepository> log;
        public SistemaDeCadastroContext context;
        public FuncionarioRepository(SistemaDeCadastroContext _context, ILogger _log)
            : base(_context)
        {
            _log = log;
            _context = context; 
            
        }
        public async Task<List<Funcionario>> GetFuncionairoByEmail(string email)
        {
            try
            {

               return await this.FindBy(c => c.Email == email.ToLower());

            }
            catch (Exception err)
            {
                this.log.LogError(err.Message);
                throw err;
            }
        }
        public async Task<List<Funcionario>> GetFuncionairoById(int id)
        {
            try
            {
                
                return await this.FindBy(c => c.Id == id);

            }
            catch (Exception err)
            {
                this.log.LogError(err.Message);
                throw err;
            }
        }
    }
}
