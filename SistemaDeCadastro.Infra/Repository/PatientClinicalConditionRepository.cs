using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientClinicalConditionRepository : BaseRepository<PatientClinicalCondition>, IPatientClinicalConditionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public PatientClinicalConditionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PatientClinicalCondition?> GetById(long id)
        {
            return await _context.PatientClinicalConditions.FirstOrDefaultAsync(pcc => pcc.Id == id);
        }

        public async Task<PatientClinicalCondition?> GetWithPatientConditionAndMedicines(long id)
        {
            return await _context.PatientClinicalConditions
                .Include(pcc => pcc.Patient)
                .Include(pcc => pcc.ClinicalCondition)
                .Include(pcc => pcc.Medicines)
                    .ThenInclude(mpcc => mpcc.Medicine)
                .FirstOrDefaultAsync(pcc => pcc.Id == id);
        }
    }
}
