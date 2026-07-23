using Azure;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Filters;
 using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public AppointmentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetById(long id)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;

            var query = from a in _context.Appointments
                         join p in _context.Patients on a.PatientId equals p.Id
                        select new AppointmentFilterDTO
                        {
                            Id = a.Id,
                            PatientId = a.PatientId,
                            PatientName = p.Name,
                            UserId = a.UserId,
                            AppointmentType = a.AppointmentType,
                            DateTime = a.DateTime,
                            Responsible = a.Responsible,
                            Status = a.Status
                        };
            if(!string.IsNullOrEmpty(filter.PatientName))
            {
                query = query.Where(a => a.PatientName.Contains(filter.PatientName));
            }
            if(filter.PatientId.HasValue)
            {
                query = query.Where(a => a.PatientId == filter.PatientId.Value);
            }
            if(!string.IsNullOrEmpty(filter.AppointmentType))
            {
                query = query.Where(a => a.AppointmentType.Contains(filter.AppointmentType));
            }
            if (filter.DateTime.HasValue)
            {
                var startDate = filter.DateTime.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.DateTime >= startDate && a.DateTime < endDate);
            }
            if (!string.IsNullOrEmpty(filter.Responsible))
            {
                query = query.Where(a => a.Responsible.Contains(filter.Responsible));
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(a => a.Status.Contains(filter.Status));
            }
            if (!string.IsNullOrEmpty(filter.Observations))
            {
                query = query.Where(a => a.Observations.Contains(filter.Observations));
            }
            var ret = new PagedAppointmentDTO();
            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Appointments = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;

        }
        public async Task<Appointment?> GetWithPatientAndUser(long id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Appointment>> GetByPatientId(long patientId)
        {
            return await _context.Appointments.Where(a => a.PatientId == patientId).ToListAsync();
        }
    }
}
