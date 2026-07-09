using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAppointmentApp
    {
        Task<List<Appointment>> GetAll();
        Task<Appointment?> GetById(long id);
        Task<ApiResponse> Create(Appointment entity);
        Task<ApiResponse> Update(Appointment entity);
        Task<ApiResponse> Delete(long id);
    }
}
