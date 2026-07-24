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

namespace SistemaDeCadastro.Infra.Repository
{
    public class CareServiceRepository : BaseRepository<CareService>, ICareServiceRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public CareServiceRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CareService?> GetById(long id)
        {
            return await _context.CareServices.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CareService?> GetWithAppointmentPatientAndUser(long id)
        {
            return await _context.CareServices
                .Include(c => c.Appointment)
                .Include(c => c.Patient)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CareService>> GetByAppointmentId(long appointmentId)
        {
            return await _context.CareServices.Where(c => c.AppointmentId == appointmentId).ToListAsync();
        }
        public async Task<PagedCareServiceDTO> GetCareServiceByFilter(CareServiceFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from c in _context.CareServices
                        join a in _context.Appointments on c.AppointmentId equals a.Id
                        join p in _context.Patients on a.PatientId equals p.Id
                        
                        select new CareServiceListDTO
                        {
                            Id = c.Id,
                            AppointmentId = a.Id,
                            PatientName = p.Name,
                            Description = c.Description,
                            AppointmentType = a.AppointmentType,
                            Referral = c.Referral,
                            Observations = c.Observations,
                            ServiceDate = c.ServiceDate,
                        };
            if (filter.AppointmentId != 0)
            {
                query = query.Where(c => c.AppointmentId == filter.AppointmentId);
            }
            if (!string.IsNullOrEmpty(filter.PatientName))
            {
                query = query.Where(c => c.PatientName.Contains(filter.PatientName));
            }
            if (!string.IsNullOrEmpty(filter.Referral))
            {
                query = query.Where(c => c.Referral.Contains(filter.Referral));
            }
            if (!string.IsNullOrEmpty(filter.AppointmentType))
            {
                query = query.Where(c => c.AppointmentType.Contains(filter.AppointmentType));
            }
            if (!string.IsNullOrEmpty(filter.AppointmentType))
            {
                query = query.Where(c => c.AppointmentType.Contains(filter.AppointmentType));
            }
            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(c => c.Description.Contains(filter.Description));
            }
            if (!string.IsNullOrEmpty(filter.Observations))
            {
                query = query.Where(c => c.Observations.Contains(filter.Observations));
            }
            if (filter.ServiceDate.HasValue)
            {
                var startDate = filter.ServiceDate.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.ServiceDate >= startDate && a.ServiceDate < endDate);
            }
            var ret = new PagedCareServiceDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.CareServices = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
