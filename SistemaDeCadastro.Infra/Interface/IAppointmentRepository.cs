using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        Task<AppointmentDTO?> GetById(long id);
        Task<Appointment?> GetWithPatientAndUser(long id);
        Task<List<Appointment>> GetByPatientId(long patientId);
        Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter);
        Task<List<AppointmentListDTO>> GetAllAppointment();

    }
}
