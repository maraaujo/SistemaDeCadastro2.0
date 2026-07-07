using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientEmployeeRepository : BaseRepository<PatientEmployee>, IPatientEmployeeRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public PatientEmployeeRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PatientEmployee?> GetById(long id)
        {
            return await _context.PatientEmployees.FirstOrDefaultAsync(pe => pe.Id == id);
        }

        public async Task<List<PatientEmployee>> GetByPatientId(long patientId)
        {
            return await _context.PatientEmployees.Where(pe => pe.PatientId == patientId).ToListAsync();
        }
    }
}
