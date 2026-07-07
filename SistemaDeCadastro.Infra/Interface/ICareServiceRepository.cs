using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface ICareServiceRepository : IBaseRepository<CareService>
    {
        Task<CareService?> GetById(long id);
        Task<CareService?> GetWithAppointmentPatientAndUser(long id);
        Task<List<CareService>> GetByAppointmentId(long appointmentId);
    }
}
