using SistemaDeCadastro.Domain.Models.Stage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientEmployeeRepository : IBaseRepository<PatientEmployee>
    {
        Task<PatientEmployee?> GetById(long id);
        Task<List<PatientEmployee>> GetByPatientId(long patientId);
    }
}
