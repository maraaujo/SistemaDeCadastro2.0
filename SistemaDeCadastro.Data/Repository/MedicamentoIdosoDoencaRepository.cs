using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Model;
using System.Runtime.CompilerServices;

namespace SistemaDeCadastro.Data.Repository
{
    public class MedicamentoIdosoDoencaRepository : BaseRepository<MedicamentoIdosoDoenca>, IMedicamentoIdosoDoencaRepository
    {
        public MedicamentoIdosoDoencaRepository(SistemaDeCadastroContext context)
            : base(context)
        {
        }

        public async Task<List<string>> GetMedicamentoByIdoso(int idoso)
        {
            return await this._context.MedicamentoIdosoDoencas.Where(c => c.Idoso.Id == idoso)
                .Select(c=> c.Medicamentos.Nome).ToListAsync();
            //retorna uam lista de medicamneto que o idoso passado como parametro toma 
        }
        


    }
}
