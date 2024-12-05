using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PosologiaRepository : BaseRepository<Posologia>, IPosologiaRepository
    {
        private readonly SistemaDeCadastroContext _context;
        public PosologiaRepository(SistemaDeCadastroContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<List<Posologia>> FindPosologiaById(int code)
        {
            return await this.FindBy(c => c.Cod == code);
        }
        public async Task<List<PosologiaDTO>> GetPosologiaByDateAndTime(int code)
        {
            var date = await this._context.Posologias
                          .Where(poso => poso.Cod == code)
                          .Select(poso => new PosologiaDTO()
                          {
                              Cod = poso.Cod,
                              DataInicial = poso.DataInicial,
                              DataFinal = poso.DataFinal,
                              ViasCod = poso.ViasCod
                          }).ToListAsync();

            return date;
        }
        //create update

    }
}
