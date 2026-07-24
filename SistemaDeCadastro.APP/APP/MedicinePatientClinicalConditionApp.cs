using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicinePatientClinicalConditionApp : IMedicinePatientClinicalConditionApp
    {
        private readonly IMedicinePatientClinicalConditionRepository _repo;
        private readonly IPatientRepository _patientRepository;

        public MedicinePatientClinicalConditionApp(IMedicinePatientClinicalConditionRepository repo, IPatientRepository patientRepository)
        {
            _repo = repo;
            _patientRepository = patientRepository;
        }

        public async Task<List<MedicinePatientClinicalCondition>> GetAll() => await _repo.GetAll();

        public async Task<MedicinePatientClinicalCondition?> GetById(long id) => (await _repo.FindBy(m => m.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try {
                var newEntity = new MedicinePatientClinicalCondition
                {
                    MedicineId = entity.MedicineId,
                    PatientClinicalConditionId = entity.PatientClinicalConditionId,
                    PrescribedDosage = entity.PrescribedDosage,
                    Frequency = entity.Frequency,
                    AdministrationTime = entity.AdministrationTime,
                    ResponsibleEmployeeId = entity.ResponsibleEmployeeId,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    Observations = entity.Observations
                };
                  await _repo.Create(newEntity);
             
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, ErrorMessage = ex.Message };
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateMedicinePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {

                var existingEntity = (await _repo.FindBy(m => m.Id == entity.Id)).FirstOrDefault();
                existingEntity.MedicineId = entity.MedicineId;
                existingEntity.PatientClinicalConditionId = entity.PatientClinicalConditionId;
                existingEntity.PrescribedDosage = entity.PrescribedDosage;
                existingEntity.Frequency = entity.Frequency;
                existingEntity.AdministrationTime = entity.AdministrationTime;
                existingEntity.ResponsibleEmployeeId = entity.ResponsibleEmployeeId;
                existingEntity.StartDate = entity.StartDate;
                existingEntity.EndDate = entity.EndDate;
                existingEntity.Observations = entity.Observations;
                await _repo.Update(existingEntity);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, ErrorMessage = ex.Message };
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(m => m.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            return await _repo.GetByPatientClinicalConditionId(patientClinicalConditionId);
        }

        //implemntar job para ficar atualizando a lista de lembretes de medicamentos, e enviar notificação para o paciente
        public async Task<List<MedicineReminderDTO>> GetMedicineReminders()
        {
            var items = await _repo.GetAll();
            var result = new List<MedicineReminderDTO>();
            foreach (var it in items)
            {
                var pcc = it.PatientClinicalCondition;
                var patient = pcc?.Patient ?? (await _patientRepository.GetPatientById(pcc?.PatientId ?? 0)).FirstOrDefault();

                var dto = new MedicineReminderDTO
                {
                    PatientId = patient?.Id ?? 0,
                    PatientName = patient?.Name ?? string.Empty,
                    MedicineName = it.Medicine?.Name ?? string.Empty,
                    Dosage = it.PrescribedDosage,
                    Frequency = it.Frequency,
                    AdministrationTime = it.AdministrationTime,
                    NextDoseDateTime = (it.AdministrationTime.HasValue) ? DateTime.Today.Add(it.AdministrationTime.Value) : DateTime.Now,
                    ResponsibleEmployeeName = it.ResponsibleEmployee?.Name ?? string.Empty,
                };
                dto.MinutesRemaining = (int)(dto.NextDoseDateTime - DateTime.Now).TotalMinutes;
                dto.AlertText = dto.MinutesRemaining <= 0 ? "Due now" : $"In {dto.MinutesRemaining} minutes";
                result.Add(dto);
            }

            return result;
        }
       public async Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter)
        {
            return await _repo.GetMedicinePatientClinicalConditionByFilter(filter);
        }
    }
}
