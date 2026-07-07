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

        public ResponsibleRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Responsible?> GetById(long id)
        {
            return await _context.Responsibles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Responsible>> GetByPatientId(long patientId)
        {
            return await _context.Responsibles.Where(r => r.PatientId == patientId).ToListAsync();
        }
    }
}
