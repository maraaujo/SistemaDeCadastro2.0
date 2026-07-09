using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
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

        public async Task<ApiResponse> Create(MedicinePatientClinicalCondition entity)
        {
            var ret = new ApiResponse();
            try { await _repo.Create(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Update(MedicinePatientClinicalCondition entity)
        {
            var ret = new ApiResponse();
            try { await _repo.Update(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
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
    }
}
