using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicinePatientClinicalConditionRepository : BaseRepository<MedicinePatientClinicalCondition>, IMedicinePatientClinicalConditionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public MedicinePatientClinicalConditionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<MedicinePatientClinicalCondition?> GetById(long id)
        {
            return await _context.MedicinePatientClinicalConditions
                .Include(mp => mp.Medicine)
                .Include(mp => mp.PatientClinicalCondition)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }

        public async Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            return await _context.MedicinePatientClinicalConditions
                .Where(mp => mp.PatientClinicalConditionId == patientClinicalConditionId)
                .Include(mp => mp.Medicine)
                .ToListAsync();
        }
    }
}
