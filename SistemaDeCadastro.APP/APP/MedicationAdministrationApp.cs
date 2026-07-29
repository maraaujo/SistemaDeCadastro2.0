using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicationAdministrationApp : IMedicationAdministrationApp
    {
        private readonly IMedicationAdministrationRepository _medicationAdministrationRepository;
        public MedicationAdministrationApp(IMedicationAdministrationRepository medicationAdministrationRepository)
        {
            this._medicationAdministrationRepository = medicationAdministrationRepository;
        }

        public async Task<List<MedicationAdministration>> GetById(long ind) =>
            await this._medicationAdministrationRepository.GetById(ind);
        public async Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter) =>
            await this._medicationAdministrationRepository.GetMedicationAdministrationByFilter(filter); 
        public async Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status) =>
            await this._medicationAdministrationRepository.GetMedicationAdministrationByStatus(status);

        public async Task<ApiResponse> Create(CreateMedicationAdministrationDTO medicationAdministration)
        {
            ApiResponse ret = new();
            try
            {
                MedicationAdministration medicationAdministration1 = new()
                {
                    MedicinePatientClinicalConditionId = medicationAdministration.MedicinePatientClinicalConditionId,
                    PatientId = medicationAdministration.PatientId,
                    EmployeeId = medicationAdministration.EmployeeId,
                    ScheduledDateTime = medicationAdministration.ScheduledDateTime,
                    AdministeredDateTime = medicationAdministration.AdministeredDateTime,
                    Status = medicationAdministration.Status,
                    Observations = medicationAdministration.Observations,
                    CreatedAt = DateTime.Now
                };
                await this._medicationAdministrationRepository.Create(medicationAdministration1);
                ret.Success = true;
                ret.Message = "Medication administration created successfully.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.Message = "An error occurred while creating the medication administration.";
            }
            return ret;
        }
        public async Task<ApiResponse> Update(UpdateMedicationAdministrationDTO medicationAdministration){

            ApiResponse ret = new();
            try
            {
                MedicationAdministration medicationAdministration1 = (await this._medicationAdministrationRepository.GetById(medicationAdministration.Id)).FirstOrDefault();
                medicationAdministration1.MedicinePatientClinicalConditionId = medicationAdministration.MedicinePatientClinicalConditionId;
                medicationAdministration1.PatientId = medicationAdministration.PatientId;
                medicationAdministration1.EmployeeId = medicationAdministration.EmployeeId;
                medicationAdministration1.ScheduledDateTime = medicationAdministration.ScheduledDateTime;
                medicationAdministration1.AdministeredDateTime = medicationAdministration.AdministeredDateTime;
                medicationAdministration1.Status = medicationAdministration.Status;
                medicationAdministration1.Observations = medicationAdministration.Observations;
              
                await this._medicationAdministrationRepository.Update(medicationAdministration1);
                ret.Success = true;
                ret.Message = "Medication administration updated successfully.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.Message = "An error occurred while updating the medication administration.";
            }
            return ret;
        }
        public async Task<ApiResponse> Delete(MedicationAdministration medicationAdministration)
        {
            ApiResponse ret = new();
            try
            {
                MedicationAdministration idToDelete = (await this._medicationAdministrationRepository.GetById(medicationAdministration.Id)).FirstOrDefault();
                await this._medicationAdministrationRepository.Delete(idToDelete);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }
    }
}
