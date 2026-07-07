using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientClinicalConditionRepository : IBaseRepository<PatientClinicalCondition>
    {
        Task<PatientClinicalCondition?> GetById(long id);
        Task<PatientClinicalCondition?> GetWithPatientConditionAndMedicines(long id);
    }
}
