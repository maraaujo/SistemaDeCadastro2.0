using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class AppointmentApp : IAppointmentApp
    {
        private readonly IAppointmentRepository _repo;
        private readonly IPatientRepository _patientRepository;
        private readonly ICurrentUserServiceContext _currentUserService;

        /// <summary>
        /// Recebe por injecao de dependencia os repositorios de agendamento e acolhido e o usuario logado.
        /// </summary>
        public AppointmentApp(IAppointmentRepository repo, IPatientRepository patientRepository, ICurrentUserServiceContext currentUserService)
        {
            _repo = repo;
            _patientRepository = patientRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Devolve todos os agendamentos visiveis para a instituicao do usuario, no formato de listagem.
        /// </summary>
        public async Task<List<AppointmentListDTO>> GetAllAppointment() => await _repo.GetAllAppointment();

        /// <summary>
        /// Busca um agendamento pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<AppointmentDTO?> GetById(long id) => await _repo.GetById(id);

        /// <summary>
        /// Cria um agendamento para um acolhido. Valida se o acolhido foi informado, se o usuario logado
        /// tem instituicao e se o acolhido pertence a essa instituicao ("Paciente nao encontrado para a instituicao...").
        /// O InstitutionId gravado vem sempre do usuario logado, nunca do DTO.
        /// </summary>
        public async Task<ApiResponse> Create(CreateAppointmentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                if (entity == null || entity.PatientId <= 0 )
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Paciente não informado";
                    return ret;
                }

                var institutionId = _currentUserService.InstitutionId;

                if (!institutionId.HasValue)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Não foi possível identificar a instituição do usuário logado.";
                    return ret;
                }

                var patient = (await _patientRepository.GetPatientById(entity.PatientId)).FirstOrDefault();

                if (patient == null || patient.InstitutionId != institutionId.Value)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Paciente não encontrado para a instituição do usuário logado.";
                    return ret;
                }

                // Atribui os valores do DTO para a entidade Appointment e forçar InstitutionId vindo do usuário logado
                var appointment = new Appointment
                {
                    UserId = entity.UserId,
                    PatientId = entity.PatientId,
                    AppointmentType = entity.AppointmentType,
                    DateTime = entity.DateTime,
                    Responsible = entity.Responsible,
                    Status = entity.Status,
                    Observations = entity.Observations,
                    InstitutionId = institutionId.Value
                };

                await _repo.Create(appointment);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Atualiza acolhido, tipo, data/hora, responsavel, status e observacoes de um agendamento existente.
        /// Retorna "Agendamento nao encontrado" se o Id nao existir e recusa trocar para um acolhido de outra instituicao.
        /// O UserId original do agendamento nao e alterado.
        /// </summary>
        public async Task<ApiResponse> Update(UpdateAppointmentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {

                var existingAppointment = (await _repo.FindBy(a => a.Id == entity.Id)).FirstOrDefault();
                if (existingAppointment == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Agendamento não encontrado";
                    return ret;
                }

                // O acolhido informado tambem precisa ser da instituicao do usuario
                // (o repositorio de pacientes ja aplica o filtro de instituicao).
                if (entity.PatientId != existingAppointment.PatientId &&
                    !await _patientRepository.Any(p => p.Id == entity.PatientId))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Paciente não encontrado para a instituição do usuário logado.";
                    return ret;
                }

                // Atualiza os campos do agendamento existente com os valores do DTO
                existingAppointment.PatientId = entity.PatientId;
                existingAppointment.AppointmentType = entity.AppointmentType;
                existingAppointment.DateTime = entity.DateTime;
                existingAppointment.Responsible = entity.Responsible;
                existingAppointment.Status = entity.Status;
                existingAppointment.Observations = entity.Observations;

                await _repo.Update(existingAppointment);
                ret.Success = true;

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Exclui o agendamento com o Id informado. Retorna Success = true mesmo se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(a => a.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve a lista paginada de agendamentos.
        /// </summary>
        public async Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter)
        {
            return await _repo.GetPagedAppointmentByFilter(filter);
        }

    }
}
