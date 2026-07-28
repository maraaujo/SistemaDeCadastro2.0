using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientEmployeeRepository : IBaseRepository<PatientEmployee>
    {
        Task<PatientEmployee?> GetById(long id);
        Task<List<PatientEmployee>> GetByPatientId(long patientId);
        Task<PagedPatientEmployeeDTO> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter);
    }
}
