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
        private readonly IPatientRepository _patientRepository;
        private readonly IEmployeeRepository _employeeRepository;
        /// <summary>
        /// Recebe por injecao de dependencia os repositorios de administracao, medicamento programado, acolhido e funcionario.
        /// </summary>
        public MedicationAdministrationApp(
            IMedicationAdministrationRepository medicationAdministrationRepository,
            IMedicinePatientClinicalConditionRepository medicinePatientClinicalConditionRepository,
            IPatientRepository patientRepository,
            IEmployeeRepository employeeRepository)
        {
            this._medicationAdministrationRepository = medicationAdministrationRepository;
            this._medicinePatientClinicalConditionRepository = medicinePatientClinicalConditionRepository;
            this._patientRepository = patientRepository;
            this._employeeRepository = employeeRepository;
        }

        // Os ids vem do cliente: confere que acolhido, medicamento programado e
        // funcionario sao da instituicao do usuario (os repositorios ja aplicam o
        // filtro de instituicao). Retorna a mensagem de erro ou null se estiver ok.
        private async Task<string?> ValidateReferences(long medicinePatientClinicalConditionId, long patientId, long? employeeId)
        {
            if (!await _patientRepository.Any(p => p.Id == patientId))
                return "Acolhido não encontrado para a instituição do usuário logado.";

            if (!await _medicinePatientClinicalConditionRepository.Any(m =>
                    m.Id == medicinePatientClinicalConditionId &&
                    m.PatientClinicalCondition.PatientId == patientId))
                return "Medicamento programado não encontrado para este acolhido.";

            if (employeeId.HasValue && !await _employeeRepository.Any(e => e.Id == employeeId.Value))
                return "Funcionário não encontrado para a instituição do usuário logado.";

            return null;
        }

        /// <summary>
        /// Devolve uma lista com a administracao de medicamento do Id informado (vazia se nao existir).
        /// </summary>
        public async Task<List<MedicationAdministration>> GetById(long ind) =>
            await this._medicationAdministrationRepository.GetById(ind);
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve o historico paginado de administracoes de medicamento.
        /// </summary>
        public async Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter) =>
            await this._medicationAdministrationRepository.GetMedicationAdministrationByFilter(filter); 
        /// <summary>
        /// Devolve as administracoes de medicamento com o status informado.
        /// </summary>
        public async Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status) =>
            await this._medicationAdministrationRepository.GetMedicationAdministrationByStatus(status);
        /// <summary>
        /// Devolve os lembretes de medicamentos (proximas doses programadas), calculados no repositorio.
        /// </summary>
        public async Task<List<MedicineReminderDTO>> GetMedicineReminders() =>
    await _medicationAdministrationRepository.GetMedicineReminders();
        /// <summary>
        /// Devolve o relatorio diario de medicacao da data de referencia, opcionalmente de um so acolhido (calculado no repositorio).
        /// </summary>
        public async Task<List<DailyMedicationReportItemDTO>> GetDailyMedicationReport(DateTime? referenceDate, long? patientId) =>
            await _medicationAdministrationRepository.GetDailyMedicationReport(referenceDate, patientId);
        /// <summary>
        /// Registra a administracao (ministracao) de um medicamento programado a um acolhido.
        /// Valida se acolhido, medicamento programado e funcionario sao da instituicao; se o horario previsto
        /// nao vier, usa hoje + horario do medicamento programado. Bloqueia duplicado no mesmo minuto
        /// ("Ja existe esse medicamento cadastrado nesse horario.", IsBusinessExcpetion = true).
        /// Horarios usam Sao Paulo; status padrao "Administrado". Dose "Nao administrado"/"Cancelado" fica sem horario de
        /// administracao. Em excecao, registra o erro no log e devolve a mensagem generica com codigo.
        /// </summary>
        public async Task<ApiResponse> Create(
     CreateMedicationAdministrationDTO medicationAdministration)
        {
            ApiResponse ret = new();

            try
            {
                var referenceError = await ValidateReferences(
                    medicationAdministration.MedicinePatientClinicalConditionId,
                    medicationAdministration.PatientId,
                    medicationAdministration.EmployeeId);
                if (referenceError != null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = referenceError;
                    ret.Message = referenceError;
                    return ret;
                }

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

                // Impede cadastrar a mesma ministracao (mesmo medicamento programado + mesmo
                // acolhido) no mesmo horario previsto (compara dentro do mesmo minuto).
                var minutoInicio = new DateTime(scheduled.Year, scheduled.Month, scheduled.Day,
                    scheduled.Hour, scheduled.Minute, 0);
                var minutoFim = minutoInicio.AddMinutes(1);

                var jaExiste = await _medicationAdministrationRepository.FindBy(a =>
                    a.MedicinePatientClinicalConditionId == medicationAdministration.MedicinePatientClinicalConditionId &&
                    a.PatientId == medicationAdministration.PatientId &&
                    a.ScheduledDateTime >= minutoInicio &&
                    a.ScheduledDateTime < minutoFim);

                if (jaExiste.Any())
                {
                    ret.Success = false;
                    ret.IsBusinessExcpetion = true;
                    ret.Message = "Já existe esse medicamento cadastrado nesse horário.";
                    ret.ErrorMessage = "Já existe esse medicamento cadastrado nesse horário.";
                    return ret;
                }

                // Status escolhido pelo usuario (Administrado, Não administrado, Atrasado,
                // Cancelado); se nao vier nenhum, assume "Administrado".
                var status = string.IsNullOrWhiteSpace(medicationAdministration.Status)
                    ? "Administrado"
                    : medicationAdministration.Status.Trim();

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

                    // Horário em que realmente foi administrado (valor enviado pelo usuário
                    // ou, se não vier, o momento atual). Dose "Não administrada" ou
                    // "Cancelada" não tem horário de administração: se tivesse, contaria
                    // como dose dada nos lembretes, no relatório do dia e na adesão.
                    AdministeredDateTime =
                        SistemaDeCadastro.Domain.Utils.MedicationStatus.IsNotAdministered(status)
                            ? null
                            : medicationAdministration.AdministeredDateTime ?? now,

                    Status = status,

                    Observations =
                        medicationAdministration.Observations ?? string.Empty,

                    CreatedAt = now
                };

                await _medicationAdministrationRepository.Create(administration);

                ret.Success = true;
                ret.Message = "Administração de medicamento registrada com sucesso.";
            }
            catch (Exception ex)
            {
                // Registra o erro no log (com codigo) em vez de descarta-lo.
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
                ret.Message = ret.ErrorMessage;
            }

            return ret;
        }
        /// <summary>
        /// Atualiza uma administracao de medicamento existente ("Administracao de medicamento nao encontrada." se nao existir),
        /// validando acolhido, medicamento programado e funcionario da instituicao. Copia os campos do DTO como vierem,
        /// sem checar duplicado de horario (dose "Nao administrado"/"Cancelado" fica sem horario de administracao).
        /// Em excecao, registra o erro no log e devolve a mensagem generica com codigo.
        /// </summary>
        public async Task<ApiResponse> Update(UpdateMedicationAdministrationDTO medicationAdministration){

            ApiResponse ret = new();
            try
            {
                MedicationAdministration medicationAdministration1 = (await this._medicationAdministrationRepository.GetById(medicationAdministration.Id)).FirstOrDefault();
                if (medicationAdministration1 == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Administração de medicamento não encontrada.";
                    ret.Message = ret.ErrorMessage;
                    return ret;
                }

                var referenceError = await ValidateReferences(
                    medicationAdministration.MedicinePatientClinicalConditionId,
                    medicationAdministration.PatientId,
                    medicationAdministration.EmployeeId);
                if (referenceError != null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = referenceError;
                    ret.Message = referenceError;
                    return ret;
                }

                medicationAdministration1.MedicinePatientClinicalConditionId = medicationAdministration.MedicinePatientClinicalConditionId;
                medicationAdministration1.PatientId = medicationAdministration.PatientId;
                medicationAdministration1.EmployeeId = medicationAdministration.EmployeeId;
                medicationAdministration1.ScheduledDateTime = medicationAdministration.ScheduledDateTime;
                // Mesma regra do cadastro: dose nao administrada/cancelada fica sem horario.
                medicationAdministration1.AdministeredDateTime =
                    SistemaDeCadastro.Domain.Utils.MedicationStatus.IsNotAdministered(medicationAdministration.Status)
                        ? null
                        : medicationAdministration.AdministeredDateTime;
                medicationAdministration1.Status = medicationAdministration.Status;
                medicationAdministration1.Observations = medicationAdministration.Observations;

                await this._medicationAdministrationRepository.Update(medicationAdministration1);
                ret.Success = true;
                ret.Message = "Administração de medicamento atualizada com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
                ret.Message = ret.ErrorMessage;
            }
            return ret;
        }
        /// <summary>
        /// Exclui a administracao de medicamento com o Id informado (so enxerga as da instituicao do usuario);
        /// retorna "Administracao de medicamento nao encontrada." se nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            ApiResponse ret = new();
            try
            {
                // O filtro de instituicao do contexto garante que so acha registro da
                // instituicao do usuario logado.
                MedicationAdministration? idToDelete = (await this._medicationAdministrationRepository.GetById(id)).FirstOrDefault();
                if (idToDelete == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Administração de medicamento não encontrada.";
                    ret.Message = ret.ErrorMessage;
                    return ret;
                }

                await this._medicationAdministrationRepository.Delete(idToDelete);
                ret.Success = true;
                ret.Message = "Administração de medicamento excluída com sucesso.";
            }
            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }
            return ret;
        }
    }
}
