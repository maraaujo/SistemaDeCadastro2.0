using Microsoft.EntityFrameworkCore.Infrastructure;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Repository
{
    public class MorbidadeRepository : BaseRepository<Morbidade>, IMorbidadaRepository
    {
        private readonly SistemaDeCadastroContext _context;
       
        public MorbidadeRepository(SistemaDeCadastroContext context
            )
            :base(context)
        {
            this._context = context;
           
        }

        public async Task<List<Morbidade>>GetMorbidadeById(int code)
        {
            return await this.FindBy(c => c.Cod == code);
        }
   
       
    }
}
