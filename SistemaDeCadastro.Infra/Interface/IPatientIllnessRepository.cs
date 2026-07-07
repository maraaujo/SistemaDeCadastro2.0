using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientIllnessRepository : IBaseRepository<PatientIllness>
    {
        Task<PatientIllness?> GetById(long id);
    }
}
