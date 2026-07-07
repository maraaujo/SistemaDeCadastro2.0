using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public PaymentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetById(long id)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetWithAppointmentAndUser(long id)
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Payment>> GetByAppointmentId(long appointmentId)
        {
            return await _context.Payments.Where(p => p.AppointmentId == appointmentId).ToListAsync();
        }
    }
}
