using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class ResponsibleRepository : BaseRepository<Responsible>, IResponsibleRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public ResponsibleRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca um responsavel pelo id; o filtro do contexto so devolve responsaveis de acolhidos da instituicao logada.</summary>
        public async Task<Responsible?> GetById(long id)
        {
            return await _context.Responsibles.FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>Lista todos os responsaveis cadastrados para o acolhido informado.</summary>
        public async Task<List<Responsible>> GetByPatientId(long patientId)
        {
            return await _context.Responsibles.Where(r => r.PatientId == patientId).ToListAsync();
        }
    }
}
