using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAppointmentApp
    {
        Task<List<Appointment>> GetAll();
        Task<Appointment?> GetById(long id);
        Task<ApiResponse> Create(CreateAppointmentDTO entity);
        Task<ApiResponse> Update(UpdateAppointmentDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter);

    }
}
