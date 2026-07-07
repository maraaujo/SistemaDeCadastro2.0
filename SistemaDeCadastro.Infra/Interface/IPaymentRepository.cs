using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<Payment?> GetById(long id);
        Task<Payment?> GetWithAppointmentAndUser(long id);
        Task<List<Payment>> GetByAppointmentId(long appointmentId);
    }
}
