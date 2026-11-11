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

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public PaymentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca um pagamento pelo id; o filtro do contexto so devolve pagamentos de agendamentos da instituicao logada.</summary>
        public async Task<Payment?> GetById(long id)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>Busca um pagamento pelo id carregando junto o agendamento e o usuario.</summary>
        public async Task<Payment?> GetWithAppointmentAndUser(long id)
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>Lista todos os pagamentos ligados ao agendamento informado.</summary>
        public async Task<List<Payment>> GetByAppointmentId(long appointmentId)
        {
            return await _context.Payments.Where(p => p.AppointmentId == appointmentId).ToListAsync();
        }
    }
}
