using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        Task<Appointment?> GetById(long id);
        Task<Appointment?> GetWithPatientAndUser(long id);
        Task<List<Appointment>> GetByPatientId(long patientId);
    }
}
