using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicinePatientClinicalConditionRepository : IBaseRepository<MedicinePatientClinicalCondition>
    {
        Task<MedicinePatientClinicalCondition?> GetById(long id);
        Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId);
        Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter);
    }
}
