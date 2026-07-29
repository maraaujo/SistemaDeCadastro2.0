using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class InternalAssistantRepository : BaseRepository<Patient>, IInternalAssistantRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public InternalAssistantRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<List<MedicationAdministrationContextDTO>> GetMedicationAdministrationContext(
     long patientId,
     DateTime referenceDate
 )
        {
            var startDate = referenceDate.Date;
            var endDate = startDate.AddDays(1);

            var query =
                from adm in _context.MedicationAdministrations.AsNoTracking()

                join patient in _context.Patients.AsNoTracking()
                    on adm.PatientId equals patient.Id

                join mpcc in _context.MedicinePatientClinicalConditions.AsNoTracking()
                    on adm.MedicinePatientClinicalConditionId equals mpcc.Id

                join medicine in _context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id

                join employee in _context.Employees.AsNoTracking()
                    on adm.EmployeeId equals employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()

                where adm.PatientId == patientId
                      && adm.ScheduledDateTime >= startDate
                      && adm.ScheduledDateTime < endDate

                select new MedicationAdministrationContextDTO
                {
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    PrescribedDosage = mpcc.PrescribedDosage,
                    Frequency = mpcc.Frequency,
                    ScheduledDateTime = adm.ScheduledDateTime,
                    AdministeredDateTime = adm.AdministeredDateTime,
                    Status = adm.Status,
                    EmployeeName = employee != null ? employee.Name : "Não informado",
                    Observations = adm.Observations
                };

            return await query
                .OrderBy(x => x.ScheduledDateTime)
                .ToListAsync();
        }
    }
}
