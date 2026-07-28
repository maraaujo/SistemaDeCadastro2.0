using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientIllnessRepository : IBaseRepository<PatientIllness>
    {
        Task<PatientIllness?> GetById(long id);
        Task<PagedPatientIllnessDTO> GetPatientIllnessByFilter(PatientIllnessFilterDTO filter);
    }
}
