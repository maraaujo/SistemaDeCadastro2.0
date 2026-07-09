using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using System.Collections.Generic;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicinePatientClinicalConditionApp
    {
        Task<List<MedicinePatientClinicalCondition>> GetAll();
        Task<MedicinePatientClinicalCondition?> GetById(long id);
        Task<ApiResponse> Create(MedicinePatientClinicalCondition entity);
        Task<ApiResponse> Update(MedicinePatientClinicalCondition entity);
        Task<ApiResponse> Delete(long id);

        Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId);
        Task<List<MedicineReminderDTO>> GetMedicineReminders();
    }
}
