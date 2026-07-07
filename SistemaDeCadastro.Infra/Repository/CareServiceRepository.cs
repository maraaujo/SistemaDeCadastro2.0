using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
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
    }
}
