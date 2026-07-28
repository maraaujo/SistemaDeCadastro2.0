using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
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
        public async Task<PagedPatientClinicalConditionDTO> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;

            var query = from p in _context.PatientClinicalConditions
                        join c in _context.ClinicalConditions on p.ClinicalConditionId equals c.Id
                        join pa in _context.Patients on p.PatientId equals pa.Id
                        select new PatientClinicalConditionListDTO
                        {
                            Id = p.Id,
                            PatientId = pa.Id,
                            PatientName = pa.Name,
                            ClinicalConditionId = c.Id,
                            ClinicalConditionName = c.Name,
                            DiagnosisDate = p.DiagnosisDate,
                            Observations = p.Observations
                        };

            if (filter.Id.HasValue)
            {
                query = query.Where(pcc => pcc.Id == filter.Id.Value);
            }
            if (filter.PatientId.HasValue)
            {
                query = query.Where(pcc => pcc.PatientId == filter.PatientId.Value);
            }
            if (filter.ClinicalConditionId.HasValue)
            {
                query = query.Where(pcc => pcc.ClinicalConditionId == filter.ClinicalConditionId.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.PatientName))
            {
                query = query.Where(a => a.PatientName.ToLower().Contains(filter.PatientName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(filter.ClinicalConditionName))
            {
                query = query.Where(a => a.ClinicalConditionName.ToLower().Contains(filter.ClinicalConditionName.ToLower()));
            }
            if (filter.DiagnosisDate.HasValue)
            {
                var startDate = filter.DiagnosisDate.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.DiagnosisDate >= startDate && a.DiagnosisDate < endDate);
            }
            var ret = new PagedPatientClinicalConditionDTO();
            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.PatientClinicalConditions= await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }


    }
   
}
