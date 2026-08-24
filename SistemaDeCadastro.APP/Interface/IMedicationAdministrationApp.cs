using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicationAdministrationApp
    {
        Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter);
        Task<List<MedicationAdministration>> GetById(long ind);
        Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status);
        Task<ApiResponse> Create(CreateMedicationAdministrationDTO medicationAdministration);
        Task<ApiResponse> Update(UpdateMedicationAdministrationDTO medicationAdministration);
        Task<ApiResponse> Delete(MedicationAdministration medicationAdministration);
        Task<List<MedicineReminderDTO>> GetMedicineReminders();

    }
}
