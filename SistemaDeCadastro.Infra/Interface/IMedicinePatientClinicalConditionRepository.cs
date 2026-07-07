using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicinePatientClinicalConditionRepository : IBaseRepository<MedicinePatientClinicalCondition>
    {
        Task<MedicinePatientClinicalCondition?> GetById(long id);
        Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId);
    }
}
