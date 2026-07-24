using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicinePatientClinicalConditionApp
    {
        Task<List<MedicinePatientClinicalCondition>> GetAll();
        Task<MedicinePatientClinicalCondition?> GetById(long id);
        Task<ApiResponse> Create(CreateMedicinePatientClinicalConditionDTO entity);
        Task<ApiResponse> Update(UpdateMedicinePatientClinicalConditionDTO entity);
        Task<ApiResponse> Delete(long id);

        Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId);
        Task<List<MedicineReminderDTO>> GetMedicineReminders();
        Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter);
    }
}
