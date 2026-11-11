using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        /// <summary>Busca um pagamento pelo id.</summary>
        Task<Payment?> GetById(long id);
        /// <summary>Busca um pagamento carregando junto o agendamento e o usuario.</summary>
        Task<Payment?> GetWithAppointmentAndUser(long id);
        /// <summary>Lista os pagamentos de um agendamento.</summary>
        Task<List<Payment>> GetByAppointmentId(long appointmentId);
    }
}
