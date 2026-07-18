using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAppointmentApp
    {
        Task<List<Appointment>> GetAll();
        Task<Appointment?> GetById(long id);
        Task<ApiResponse> Create(CreateAppointmentDTO entity);
        Task<ApiResponse> Update(UpdateAppointmentDTO entity);
        Task<ApiResponse> Delete(long id);
    }
}
