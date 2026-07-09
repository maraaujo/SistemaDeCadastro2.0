using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicinePatientClinicalConditionRepository
        : BaseRepository<MedicinePatientClinicalCondition>, IMedicinePatientClinicalConditionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public MedicinePatientClinicalConditionRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Da tabela medicine patient clinical condition,
        /// pegar o registro pelo id, e incluir as informações do medicine, responsibleEmployee e patientClinicalCondition
        public async Task<MedicinePatientClinicalCondition?> GetById(long id)
        {
            return await _context.MedicinePatientClinicalConditions
                .Include(mp => mp.Medicine)
                .Include(mp => mp.ResponsibleEmployee)
                .Include(mp => mp.PatientClinicalCondition)
                    .ThenInclude(pc => pc.Patient)
                .Include(mp => mp.PatientClinicalCondition)
                    .ThenInclude(pc => pc.ClinicalCondition)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }
        //da tabela medicine patient clinical condition,
        /// <summary>
        /// pegar todos os registros que tenham o mesmo patientClinicalConditionId, 
        /// e incluir as informações do medicine, responsibleEmployee e patientClinicalCondition
        /// </summary>
        /// <param name="patientClinicalConditionId"></param>
        /// <returns></returns>

        public async Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            return await _context.MedicinePatientClinicalConditions
                .Where(mp => mp.PatientClinicalConditionId == patientClinicalConditionId)
                .Include(mp => mp.Medicine)
                .Include(mp => mp.ResponsibleEmployee)
                .Include(mp => mp.PatientClinicalCondition)
                    .ThenInclude(pc => pc.Patient)
                .ToListAsync();
        }
    }
}