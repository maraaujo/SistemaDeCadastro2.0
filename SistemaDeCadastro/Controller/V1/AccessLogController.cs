using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;


namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccessLogController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IAccessLogApp _app;

        public AccessLogController(IConfiguration configuration, IAccessLogApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }
        // POST como os demais filtros do sistema (o front ja chama via POST; um GET
        // com filtro no corpo nao e enviado pelo navegador).
        [HttpPost("GetAccessLogsByFilter")]
        public async Task<IActionResult> GetAccessLogsByFilter(AccessLogFilterDTO filter)
        {
            var items = await _app.GetAccessLogsByFilter(filter);
            return Ok(items);
        }

        [HttpGet("GetAccessLogById")]
        public async Task<IActionResult> GetAccessLogById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllAccessLogs")]
        public async Task<IActionResult> GetAllAccessLogs()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        // Logs sao a trilha de auditoria: criar/editar/apagar manualmente fica restrito
        // ao administrador da plataforma, para ninguem forjar ou apagar registros.
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateAccessLog")]
        public async Task<IActionResult> CreateAccessLog(CreateAccessLogDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        // Logs sao a trilha de auditoria: criar/editar/apagar manualmente fica restrito
        // ao administrador da plataforma, para ninguem forjar ou apagar registros.
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAccessLog(UpdateAccessLogDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        // Logs sao a trilha de auditoria: criar/editar/apagar manualmente fica restrito
        // ao administrador da plataforma, para ninguem forjar ou apagar registros.
        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("DeleteAccessLog/{idAccessLog}")]
        public async Task<IActionResult> DeleteAccessLog(long idAccessLog)
        {
            var ret = await _app.Delete(idAccessLog);
            return Ok(ret);
        }
    }
}
