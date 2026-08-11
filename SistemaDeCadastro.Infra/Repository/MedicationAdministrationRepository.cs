using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicationAdministrationRepository : BaseRepository<MedicationAdministration>, IMedicationAdministrationRepository
    {
        private readonly SistemaDeCadastroContext _context;
        public MedicationAdministrationRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<MedicationAdministration>> GetById(long ind)
        {
            return await this.FindBy(c => c.Id == ind);
        }
        public async Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status)
        {
            return await this.FindBy(c => c.Status == status);
        }

        public async Task Create(CreateMedicationAdministrationDTO medicationAdministration)
        {
            await this.Create(medicationAdministration);
        }

        public async Task Update(UpdateMedicationAdministrationDTO medicationAdministration)
        {
            await this.Update(medicationAdministration);
        }
        public async Task Delete(MedicationAdministration medicationAdministration)
        {
            
           
                await this.Delete(medicationAdministration);
        

        }
        public async Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page; 
            var query = from m in _context.MedicationAdministrations
                        join p in _context.Patients on m.PatientId equals p.Id
                        join e in _context.Employees on m.EmployeeId equals e.Id 
                        select new MedicationAdministrationDTO
                        {
                            Id = m.Id,
                            MedicinePatientClinicalConditionId = m.MedicinePatientClinicalConditionId,
                            PatientId = m.PatientId,
                            NamePatient = p.Name,
                            EmployeeId = m.EmployeeId,
                            EmployeeName = e.Name,
                            ScheduledDateTime = m.ScheduledDateTime,
                            AdministeredDateTime = m.AdministeredDateTime,
                            Status = m.Status,
                            Observations = m.Observations,
                            CreatedAt = m.CreatedAt
                        };
            if(filter.Id.HasValue)
            {
                query = query.Where(c => c.Id == filter.Id.Value);
            }
            if(!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(c => c.Status == filter.Status);
            }
            if(filter.PatientId.HasValue)
            {
                query = query.Where(c => c.PatientId == filter.PatientId.Value);
            }
            if(filter.EmployeeId.HasValue) {
                query = query.Where(c => c.EmployeeId == filter.EmployeeId.Value);
            }
            if (filter.ScheduledDateTime.HasValue)
            {

                var startTime = filter.ScheduledDateTime.Value;
                var endTime = startTime.Add(TimeSpan.FromMinutes(1));

                query = query.Where(a => a.ScheduledDateTime >= startTime && a.ScheduledDateTime < endTime);
            }
            if (filter.AdministeredDateTime.HasValue)
            {
                var startTime = filter.AdministeredDateTime.Value;
                var endTime = startTime.Add(TimeSpan.FromMinutes(1));
                query = query.Where(a => a.AdministeredDateTime >= startTime && a.AdministeredDateTime < endTime);
            }
            if (!string.IsNullOrEmpty(filter.Observations))
            {
                query = query.Where(c => c.Observations == filter.Observations);    
            }
            var ret = new PagedMedicationAdministrationDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.MedicationAdministration = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
