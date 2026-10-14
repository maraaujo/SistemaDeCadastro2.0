using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicationAdministrationApp : IMedicationAdministrationApp
    {
        private readonly IMedicationAdministrationRepository _medicationAdministrationRepository;
        private readonly IMedicinePatientClinicalConditionRepository _medicinePatientClinicalConditionRepository;
        public MedicationAdministrationApp(
            IMedicationAdministrationRepository medicationAdministrationRepository,
            IMedicinePatientClinicalConditionRepository medicinePatientClinicalConditionRepository)
        {
            this._medicationAdministrationRepository = medicationAdministrationRepository;
            this._medicinePatientClinicalConditionRepository = medicinePatientClinicalConditionRepository;
        }

        public async Task<List<MedicationAdministration>> GetById(long ind) =>
            await this._medicationAdministrationRepository.GetById(ind);
        public async Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter) =>
            await this._medicationAdministrationRepository.GetMedicationAdministrationByFilter(filter); 
        public async Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status) =>
            await this._medicationAdministrationRepository.GetMedicationAdministrationByStatus(status);
        public async Task<List<MedicineReminderDTO>> GetMedicineReminders() =>
    await _medicationAdministrationRepository.GetMedicineReminders();
        public async Task<List<DailyMedicationReportItemDTO>> GetDailyMedicationReport(DateTime? referenceDate, long? patientId) =>
            await _medicationAdministrationRepository.GetDailyMedicationReport(referenceDate, patientId);
        public async Task<ApiResponse> Create(
     CreateMedicationAdministrationDTO medicationAdministration)
        {
            ApiResponse ret = new();

            try
            {
                // Usa o horario de parede de Sao Paulo, independente do fuso do servidor
                // (ex.: UTC no Cloud Run), para que os registros batam com o horario local.
                var now = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo();

                // Horario previsto da dose. Deve vir do NextDoseDateTime do lembrete; se nao
                // vier (0001-01-01 ou 00:00), deriva do horario planejado do medicamento
                // programado (hoje + horario_administracao) para a tela de historico nao
                // ficar sem o "horario previsto".
                var scheduled = medicationAdministration.ScheduledDateTime;
                if (scheduled == default || scheduled.TimeOfDay == TimeSpan.Zero)
                {
                    var mpcc = await _medicinePatientClinicalConditionRepository
                        .GetById(medicationAdministration.MedicinePatientClinicalConditionId);

                    if (mpcc?.AdministrationTime != null)
                    {
                        var baseDate = scheduled == default ? now.Date : scheduled.Date;
                        scheduled = baseDate.Add(mpcc.AdministrationTime.Value);
                    }
                    else if (scheduled == default)
                    {
                        scheduled = now;
                    }
                }

                MedicationAdministration administration = new()
                {
                    MedicinePatientClinicalConditionId =
                        medicationAdministration.MedicinePatientClinicalConditionId,

                    PatientId =
                        medicationAdministration.PatientId,

                    EmployeeId =
                        medicationAdministration.EmployeeId,

                    // Horário previsto da dose.
                    ScheduledDateTime = scheduled,

                    // Horário em que realmente foi administrado (usa o valor enviado
                    // pelo usuário; se não vier, assume o momento atual).
                    AdministeredDateTime =
                        medicationAdministration.AdministeredDateTime ?? now,

                    // Usa o status escolhido pelo usuário (Administrado, Não administrado,
                    // Atrasado, Cancelado); se não vier nenhum, assume "Administered".
                    Status =
                        string.IsNullOrWhiteSpace(medicationAdministration.Status)
                            ? "Administered"
                            : medicationAdministration.Status,

                    Observations =
                        medicationAdministration.Observations ?? string.Empty,

                    CreatedAt = now
                };

                await _medicationAdministrationRepository.Create(administration);

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
