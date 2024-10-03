using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicamentoMorbidadeRepository : BaseRepository<MedicamentoMorbidade>
    {
        SistemaDeCadastroContext _context;
        public MedicamentoMorbidadeRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Medicamento>> GetMedicamentosPorMorbidade(int morbidadeId)
        {
            return await _context.MedicamentoMorbidades.
                Where(c => c.MorbidadeCod == morbidadeId)
                .Include(m => m.Medicamento)
                .Select(m => m.Medicamento)
                .ToListAsync();

        }

        public async Task<List<Morbidade>> GetMorbidadePorMedicamento(int medicamentoId)
        {
            return await _context.MedicamentoMorbidades.
               Where(c => c.MedicamentoCod == medicamentoId)
               .Include(m => m.Morbidade)
               .Select(m => m.Morbidade)
               .ToListAsync();
        } 
        //
       
    }
}

