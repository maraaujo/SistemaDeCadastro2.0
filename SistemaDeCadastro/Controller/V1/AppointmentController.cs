using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IAppointmentApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de agendamentos).
        /// </summary>
        public AppointmentController(IConfiguration configuration, IAppointmentApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/Appointment/GetAppointmentById/{id} - qualquer usuario autenticado.
        /// Retorna o agendamento pelo id (null se nao existir ou se for de outra instituicao,
        /// por causa do filtro por instituicao do DbContext).
        /// </summary>
        [HttpGet("GetAppointmentById/{id}")]
        public async Task<IActionResult> GetAppointmentById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/Appointment/GetAllAppointments - qualquer usuario autenticado.
        /// Retorna todos os agendamentos da instituicao do usuario logado, no formato de listagem.
        /// </summary>
        [HttpGet("GetAllAppointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var items = await _app.GetAllAppointment();
            return Ok(items);
        }

        /// <summary>
        /// POST api/Appointment/CreateAppointment - qualquer usuario autenticado.
        /// Cria um agendamento para um acolhido da instituicao do usuario logado (a instituicao gravada
        /// vem do usuario, nao do corpo). Retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreateAppointment")]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDTO entity)
        {
            // TODO: Ajustar IAppointmentApp e AppointmentApp para receber CreateAppointmentDTO
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Appointment/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o agendamento com os dados do corpo (o id usado e o do DTO, nao o da rota).
        /// Recusa trocar para acolhido de outra instituicao. Retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAppointment(UpdateAppointmentDTO entity)
        {
            // TODO: Ajustar IAppointmentApp e AppointmentApp para receber UpdateAppointmentDTO
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Appointment/DeleteAppointment/{idAppointment} - qualquer usuario autenticado.
        /// Exclui o agendamento informado; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpDelete("DeleteAppointment/{idAppointment}")]
        public async Task<IActionResult> DeleteAppointment(long idAppointment)
        {
            var ret = await _app.Delete(idAppointment);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/Appointment/GetPagedAppointmentByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de agendamentos que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetPagedAppointmentByFilter")]
        public async Task<IActionResult> GetPagedAppointmentByFilter(AppointmentFilterDTO filter)
        {
            var pagedAppointments = await _app.GetPagedAppointmentByFilter(filter);
            return Ok(pagedAppointments);
        }
    }
}
