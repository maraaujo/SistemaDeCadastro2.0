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

        public async Task<List<MedicineReminderDTO>> GetMedicineReminders()
        {
            var now = DateTime.Now;
            var today = now.Date;

            /*
                Busca as administrações registradas hoje.

                Aqui usamos tanto ScheduledDateTime quanto AdministeredDateTime porque,
                em alguns registros antigos, o ScheduledDateTime pode ter sido salvo como
                00:00:00, mas o AdministeredDateTime está correto.
            */
            var administrationsToday = await _context.MedicationAdministrations
                .AsNoTracking()
                .Where(x =>
                    x.ScheduledDateTime.Date == today ||
                    (x.AdministeredDateTime != null && x.AdministeredDateTime.Value.Date == today)
                )
                .ToListAsync();

            /*
                Busca todos os medicamentos programados para hoje.

                Importante:
                - StartDate define quando o medicamento começa a aparecer.
                - EndDate define quando ele para de aparecer.
                - Se EndDate for null, é considerado uso contínuo.
            */
            var query =
                from mpcc in _context.MedicinePatientClinicalConditions.AsNoTracking()

                join medicine in _context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id

                join pcc in _context.PatientClinicalConditions.AsNoTracking()
                    on mpcc.PatientClinicalConditionId equals pcc.Id

                join patient in _context.Patients.AsNoTracking()
                    on pcc.PatientId equals patient.Id

                join employee in _context.Employees.AsNoTracking()
                    on mpcc.ResponsibleEmployeeId equals (long?)employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()

                where mpcc.AdministrationTime != null
                      && (mpcc.StartDate == null || mpcc.StartDate.Value.Date <= today)
                      && (mpcc.EndDate == null || mpcc.EndDate.Value.Date >= today)

                select new
                {
                    MedicinePatientClinicalConditionId = mpcc.Id,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    Dosage = mpcc.PrescribedDosage,
                    Frequency = mpcc.Frequency,
                    AdministrationTime = mpcc.AdministrationTime,
                    StartDate = mpcc.StartDate,
                    EndDate = mpcc.EndDate,
                    ResponsibleEmployeeName = employee != null ? employee.Name : "Não informado"
                };

            var items = await query.ToListAsync();

                        var result = items
                            .Select(item =>
                            {
                                /*
                                    Monta a data/hora prevista real.

                                    Exemplo:
                                    today = 2026-08-24 00:00:00
                                    AdministrationTime = 10:45:00

                                    Resultado:
                                    nextDoseDateTime = 2026-08-24 10:45:00
                                */
                                var nextDoseDateTime = today.Add(item.AdministrationTime.Value);

                                /*
                                    Verifica se este medicamento programado já foi administrado hoje.

                                    item = medicamento programado que aparece em "Próximos Medicamentos"
                                    a = registro já salvo em "Administração de Medicamentos"

                                    Não vamos comparar o horário exato aqui, porque seus registros antigos
                                    salvaram ScheduledDateTime como 00:00:00.

                                    Então a regra fica:
                                    mesmo id_medicamento_acond + mesmo paciente + administrado hoje
                                    → já foi administrado
                                */
                                var alreadyAdministered = administrationsToday.Any(a =>
                                     a.MedicinePatientClinicalConditionId == item.MedicinePatientClinicalConditionId &&
                                     a.PatientId == item.PatientId &&
                                     (
                                         a.AdministeredDateTime != null ||
                                         (!string.IsNullOrWhiteSpace(a.Status) &&
                                          (
                                              a.Status.Equals("Administrado", StringComparison.OrdinalIgnoreCase) ||
                                              a.Status.Equals("Administrada", StringComparison.OrdinalIgnoreCase) ||
                                              a.Status.Equals("Realizado", StringComparison.OrdinalIgnoreCase) ||
                                              a.Status.Equals("Realizada", StringComparison.OrdinalIgnoreCase) ||
                                              a.Status.Equals("Ministrado", StringComparison.OrdinalIgnoreCase) ||
                                              a.Status.Equals("Ministrada", StringComparison.OrdinalIgnoreCase) ||
                                              a.Status.Equals("Administered", StringComparison.OrdinalIgnoreCase)
                                          ))
                                     )
                                 );

                                /*
                                    Se já foi administrado hoje, não deve aparecer mais na tela
                                    de Próximos Medicamentos.
                                */
                                if (alreadyAdministered)
                        return null;

                    var minutesRemaining = (int)(nextDoseDateTime - now).TotalMinutes;

                    string alertText;

                    if (minutesRemaining < 0)
                    {
                        alertText = $"Atrasado há {Math.Abs(minutesRemaining)} minutos";
                    }
                    else if (minutesRemaining == 0)
                    {
                        alertText = "Administrar agora";
                    }
                    else
                    {
                        alertText = $"Em {minutesRemaining} minutos";
                    }

                    return new MedicineReminderDTO
                    {
                        MedicinePatientClinicalConditionId = item.MedicinePatientClinicalConditionId,
                        PatientId = item.PatientId,
                        PatientName = item.PatientName,
                        MedicineName = item.MedicineName,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        AdministrationTime = item.AdministrationTime,
                        NextDoseDateTime = nextDoseDateTime,
                        MinutesRemaining = minutesRemaining,
                        AlertText = alertText,
                        ResponsibleEmployeeName = item.ResponsibleEmployeeName
                    };
                })
                .Where(x => x != null)
                .OrderBy(x => x.MinutesRemaining)
                .ToList();

            return result;
        }
    }
}
