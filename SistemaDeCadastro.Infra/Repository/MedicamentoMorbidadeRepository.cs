using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using System.Linq.Expressions;


namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicamentoMorbidadeRepository : BaseRepository<MedicamentoMorbidade>, IMedicamentoMorbidadeRepository
    {
        private readonly SistemaDeCadastroContext _context;
        public MedicamentoMorbidadeRepository(SistemaDeCadastroContext context) : base(context)
        {
            this._context = context;
        }

       
        public async Task<List<MedicamentoMorbidadeDTO>> GetMedicmaentoByMorbidade(int code)
        {
            var morb = await (from mediMorb in this._context.MedicamentoMorbidades
                              join medi in this._context.Medicamentos
                              on mediMorb.MedicamentoCod equals medi.Cod
                              join morbi in this._context.Morbidades
                              on mediMorb.MorbidadeCod equals morbi.Cod
                              where morbi.Cod == code
                              select new MedicamentoMorbidadeDTO()
                              {
                                  NomeMedicamento = medi.Nome,
                                  NomeMorbidade = morbi.Nome,
                                  MedicamentoCod = medi.Cod,
                                  MorbidadeCod = morbi.Cod
                              }
                              ).ToListAsync();
            return morb; 
        }
        public async Task<List<MedicamentoMorbidadeDTO>> GetMorbidadeByMedicamento(int code)
        {
            var morb = await (from mediMorb in this._context.MedicamentoMorbidades
                              join medi in this._context.Medicamentos
                              on mediMorb.MedicamentoCod equals medi.Cod
                              join morbi in this._context.Morbidades
                              on mediMorb.MorbidadeCod equals morbi.Cod
                              where medi.Cod == code
                              select new MedicamentoMorbidadeDTO()
                              {
                                  NomeMedicamento = medi.Nome,
                                  NomeMorbidade = morbi.Nome,
                                  MedicamentoCod = medi.Cod,
                                  MorbidadeCod = morbi.Cod
                              }
                              ).ToListAsync();
            return morb; 
        }

        public Task Update(MedicamentoMorbidade entity)
        {
            throw new NotImplementedException();
        }

        Task<List<MedicamentoMorbidade>> IBaseRepository<MedicamentoMorbidade>.GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
