using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
        public async Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from mp in _context.MedicinePatientClinicalConditions
                                    join m in _context.Medicines on mp.MedicineId equals m.Id
                                    join pc in _context.PatientClinicalConditions on mp.PatientClinicalConditionId equals pc.Id
                                    join p in _context.Patients on pc.PatientId equals p.Id
                                    join c in _context.ClinicalConditions on pc.ClinicalConditionId equals c.Id
                                    join e in _context.Employees on mp.ResponsibleEmployeeId equals e.Id into employeeGroup
                                    from e in employeeGroup.DefaultIfEmpty()
                                    select new MedicinePatientClinicalConditionListDTO
                        {
                            Id = mp.Id,
                            PatientName = p.Name,
                            PatientId = p.Id,
                            MedicineId = m.Id,
                            MedicineName = m.Name,
                            ClinicalConditionName = pc.ClinicalCondition.Name,
                            PrescribedDosage = mp.PrescribedDosage,
                            Frequency = mp.Frequency,
                            AdministrationTime = mp.AdministrationTime,
                            ResponsibleEmployeeName = e.Name,
                            StartDate = mp.StartDate,
                            EndDate = mp.EndDate,
                            Observations = mp.Observations,
                            PatientClinicalConditionId = pc.Id,
                            ResponsibleEmployeeId = e.Id
                        };
            if (filter.PatientId.HasValue)
            {
                query = query.Where(mp => mp.PatientId == filter.PatientId.Value);
            }

            if (filter.MedicineId.HasValue)
            {
                query = query.Where(mp => mp.MedicineId == filter.MedicineId.Value);
            }

            if (filter.PatientClinicalConditionId.HasValue)
            {
                query = query.Where(mp => mp.PatientClinicalConditionId == filter.PatientClinicalConditionId.Value);
            }

            if (filter.ResponsibleEmployeeId.HasValue)
            {
                query = query.Where(mp => mp.ResponsibleEmployeeId == filter.ResponsibleEmployeeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.PatientName))
            {
                query = query.Where(mp => mp.PatientName.Contains(filter.PatientName));
            }

            if (!string.IsNullOrWhiteSpace(filter.MedicineName))
            {
                query = query.Where(mp => mp.MedicineName.Contains(filter.MedicineName));
            }

            if (!string.IsNullOrWhiteSpace(filter.ClinicalConditionName))
            {
                query = query.Where(mp => mp.ClinicalConditionName.Contains(filter.ClinicalConditionName));
            }

            if (!string.IsNullOrWhiteSpace(filter.ResponsibleEmployeeName))
            {
                query = query.Where(mp => mp.ResponsibleEmployeeName.Contains(filter.ResponsibleEmployeeName));
            }

            if (filter.AdministrationTime.HasValue)
            {
                var startTime = filter.AdministrationTime.Value;
                var endTime = startTime.Add(TimeSpan.FromMinutes(1));

                query = query.Where(a => a.AdministrationTime >= startTime && a.AdministrationTime < endTime);
            }
            var ret = new PagedMedicinePatientClinicalConditionDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.MedicinePatientClinicalConditions  = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
    }