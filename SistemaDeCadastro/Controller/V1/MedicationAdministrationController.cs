using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicationAdministrationController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IMedicationAdministrationApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de administracao de medicamentos).
        /// </summary>
        public MedicationAdministrationController(IConfiguration configuration, IMedicationAdministrationApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/MedicationAdministration/GetMedicationAdministrationById?id= - qualquer usuario autenticado.
        /// Retorna uma lista com o registro de administracao de medicamento do id (vazia se nao existir
        /// ou se for de outra instituicao).
        /// </summary>
        [HttpGet("GetMedicationAdministrationById")]
        public async Task<IActionResult> GetMedicationAdministrationById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        //[HttpGet("GetAllMedicationAdministrations")]
        //public async Task<IActionResult> GetAllMedicationAdministrations()
        //{
        //    var items = await _app.GetAll();
        //    return Ok(items);
        //}

        /// <summary>
        /// POST api/MedicationAdministration/CreateMedicationAdministration - qualquer usuario autenticado.
        /// Registra que uma dose foi (ou nao) administrada. Confere se acolhido, medicamento programado e
        /// funcionario sao da instituicao, completa horario previsto/administrado com o horario de Sao Paulo
        /// e recusa duplicidade no mesmo minuto. Retorna ApiResponse com Success/Message.
        /// </summary>
        [HttpPost("CreateMedicationAdministration")]
        public async Task<IActionResult> CreateMedicationAdministration(CreateMedicationAdministrationDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/MedicationAdministration/Update/{id} - qualquer usuario autenticado (o id usado e o do DTO).
        /// Atualiza o registro de administracao apos validar as referencias da instituicao. Retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMedicationAdministration(UpdateMedicationAdministrationDTO entity)
        {
            // TODO: Ajustar IMedicationAdministrationApp e MedicationAdministrationApp para receber UpdateMedicationAdministrationDTO
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        // Recebe so o id da rota (antes esperava o objeto inteiro no corpo de um GET).
        // Aceita GET, padrao dos outros deletes do backend, e DELETE, que e o que o
        // medication-administrations-api.js do front envia.
        /// <summary>
        /// GET ou DELETE api/MedicationAdministration/DeleteMedicationAdministration/{id} - usuario autenticado.
        /// Exclui o registro (so encontra registros da instituicao do usuario); retorna ApiResponse.
        /// </summary>
        [HttpGet("DeleteMedicationAdministration/{idMedicationAdministration}")]
        [HttpDelete("DeleteMedicationAdministration/{idMedicationAdministration}")]
        public async Task<IActionResult> DeleteMedicationAdministration(long idMedicationAdministration)
        {
            var ret = await _app.Delete(idMedicationAdministration);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/MedicationAdministration/GetPagedMedicationAdministrationByFilter - usuario autenticado.
        /// Retorna o historico paginado de administracoes que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetPagedMedicationAdministrationByFilter")]
        public async Task<IActionResult> GetPagedMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter)
        {
            var pagedMedicationAdministrations = await _app.GetMedicationAdministrationByFilter(filter);
            return Ok(pagedMedicationAdministrations);
        }
        /// <summary>
        /// GET api/MedicationAdministration/GetMedicineReminders - qualquer usuario autenticado.
        /// Retorna os lembretes de medicamentos (proximas doses) dos acolhidos da instituicao do usuario.
        /// </summary>
        [HttpGet("GetMedicineReminders")]
        public async Task<IActionResult> GetMedicineReminders()
        {
            var ret = await _app.GetMedicineReminders();
            return Ok(ret);
        }

        // Relatorio de medicamentos do dia: todos os medicamentos programados para a data
        // informada (ou hoje) com o status (Administrado / Atrasado / Pendente).
        // Opcionalmente filtra por acolhido (patientId).
        /// <summary>
        /// GET api/MedicationAdministration/GetDailyMedicationReport?date=&amp;patientId= - usuario autenticado.
        /// Retorna a lista de itens do relatorio diario de medicamentos.
        /// </summary>
        [HttpGet("GetDailyMedicationReport")]
        public async Task<IActionResult> GetDailyMedicationReport(DateTime? date = null, long? patientId = null)
        {
            var ret = await _app.GetDailyMedicationReport(date, patientId);
            return Ok(ret);
        }
    }
}
