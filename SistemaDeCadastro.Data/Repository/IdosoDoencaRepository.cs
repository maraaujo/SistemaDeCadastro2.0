using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Data.Repository
{
    public class IdosoDoencaRepository : BaseRepository<IdosoDoenca>, IIdosoDoencaRepository
    {
        public IdosoDoencaRepository(SistemaDeCadastroContext context)
            : base(context)
        {

        }
        //todo os idosos com tal doenca
        public async Task<List<string>> GetByDoenca(int doenca)
        {
            //return await this.FindBy(c => c.Doencas.Nome == doenca); 
            return await this._context.IdososDoenca.Where(c => c.Doencas.Id == doenca)
                .Select(c => c.Idosos.Nome).ToListAsync();
            //onde o nome da doenca for igual ao que foi passado como parametro, selecione o nome do idoso
        }
       
    }

}
