using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface ICareServiceRepository : IBaseRepository<CareService>
    {
        Task<CareService?> GetById(long id);
        Task<CareService?> GetWithAppointmentPatientAndUser(long id);
        Task<List<CareService>> GetByAppointmentId(long appointmentId);
        Task<PagedCareServiceDTO> GetCareServiceByFilter(CareServiceFilterDTO filter);
    }
}
