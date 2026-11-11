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

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de logs de acesso).
        /// </summary>
        public AccessLogController(IConfiguration configuration, IAccessLogApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }
        // POST como os demais filtros do sistema (o front ja chama via POST; um GET
        // com filtro no corpo nao e enviado pelo navegador).
        /// <summary>
        /// POST api/AccessLog/GetAccessLogsByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de logs de acesso/auditoria que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetAccessLogsByFilter")]
        public async Task<IActionResult> GetAccessLogsByFilter(AccessLogFilterDTO filter)
        {
            var items = await _app.GetAccessLogsByFilter(filter);
            return Ok(items);
        }

        /// <summary>
        /// GET api/AccessLog/GetAccessLogById?id= - qualquer usuario autenticado.
        /// Retorna um log de acesso pelo id (null se nao existir).
        /// </summary>
        [HttpGet("GetAccessLogById")]
        public async Task<IActionResult> GetAccessLogById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/AccessLog/GetAllAccessLogs - qualquer usuario autenticado.
        /// Retorna todos os logs de acesso, sem paginacao.
        /// </summary>
        [HttpGet("GetAllAccessLogs")]
        public async Task<IActionResult> GetAllAccessLogs()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        // Logs sao a trilha de auditoria: criar/editar/apagar manualmente fica restrito
        // ao administrador da plataforma, para ninguem forjar ou apagar registros.
        /// <summary>
        /// POST api/AccessLog/CreateAccessLog - somente admin da plataforma (policy "SaasAdmin").
        /// Cria manualmente um registro de log; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateAccessLog")]
        public async Task<IActionResult> CreateAccessLog(CreateAccessLogDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        // Logs sao a trilha de auditoria: criar/editar/apagar manualmente fica restrito
        // ao administrador da plataforma, para ninguem forjar ou apagar registros.
        /// <summary>
        /// PUT api/AccessLog/Update/{id} - somente admin da plataforma (policy "SaasAdmin").
        /// Atualiza um log com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAccessLog(UpdateAccessLogDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        // Logs sao a trilha de auditoria: criar/editar/apagar manualmente fica restrito
        // ao administrador da plataforma, para ninguem forjar ou apagar registros.
        /// <summary>
        /// DELETE api/AccessLog/DeleteAccessLog/{idAccessLog} - somente admin da plataforma (policy "SaasAdmin").
        /// Exclui o log informado; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteAccessLog/{idAccessLog}")]
        public async Task<IActionResult> DeleteAccessLog(long idAccessLog)
        {
            var ret = await _app.Delete(idAccessLog);
            return Ok(ret);
        }
    }
}
