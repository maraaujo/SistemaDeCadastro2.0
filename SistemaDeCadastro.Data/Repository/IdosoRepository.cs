using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;



namespace SistemaDeCadastro.Data.Repository
{
    public class IdosoRepository : BaseRepository<Idoso>,  IIdosoRepository //interface
    {
        private readonly SistemaDeCadastroContext context;

        public IdosoRepository(SistemaDeCadastroContext _context)
            : base(_context)
        {
            this.context = _context;
        }
        //criarAdiminRepository
          
       
      
     
      
    }
}
