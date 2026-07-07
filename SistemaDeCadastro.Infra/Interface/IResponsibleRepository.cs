using SistemaDeCadastro.Domain.Models.Stage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IResponsibleRepository : IBaseRepository<Responsible>
    {
        Task<Responsible?> GetById(long id);
        Task<List<Responsible>> GetByPatientId(long patientId);
    }
}
