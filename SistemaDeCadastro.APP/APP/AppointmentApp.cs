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

        public AppointmentApp(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Appointment>> GetAll() => await _repo.GetAll();

        public async Task<Appointment?> GetById(long id) => (await _repo.FindBy(a => a.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateAppointmentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                if (entity == null || entity.PatientId <= 0 )
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Paciente não encontrado";
                    return ret;
                }
                //aqui eu estou atribuindo os valores do DTO para a entidade Appointment, que será salva no banco de dados
                var appointment = new Appointment
                {
                    PatientId = entity.PatientId,
                    AppointmentType = entity.AppointmentType,
                    DateTime = entity.DateTime,
                    Responsible = entity.Responsible,
                    Status = entity.Status,
                    Observations = entity.Observations
                };
                await _repo.Create(appointment);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(a => a.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
        public async Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter)
        {
            return await _repo.GetPagedAppointmentByFilter(filter);
        }

    }
}
