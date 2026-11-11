using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvailablePermissionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IAvailablePermissionApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de permissoes disponiveis).
        /// </summary>
        public AvailablePermissionController(IConfiguration configuration, IAvailablePermissionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/AvailablePermission/GetAvailablePermissionById?id= - qualquer usuario autenticado.
        /// Retorna uma permissao do catalogo de permissoes disponiveis (null se nao existir).
        /// </summary>
        [HttpGet("GetAvailablePermissionById")]
        public async Task<IActionResult> GetAvailablePermissionById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/AvailablePermission/GetAllAvailablePermission - qualquer usuario autenticado.
        /// Retorna todo o catalogo de permissoes que podem ser atribuidas aos usuarios.
        /// </summary>
        [HttpGet("GetAllAvailablePermission")]
        public async Task<IActionResult> GetAllAvailablePermission()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/AvailablePermission/CreateAvailablePermission - somente admin da plataforma (policy "SaasAdmin").
        /// Cadastra uma nova permissao no catalogo; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateAvailablePermission")]
        public async Task<IActionResult> CreateAvailablePermission(CreateAvailablePermissionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/AvailablePermission/Update/{id} - somente admin da plataforma (policy "SaasAdmin").
        /// Atualiza a permissao com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAvailablePermission(UpdateAvailablePermissionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/AvailablePermission/DeleteAvailablePermission/{idAvailablePermission} - somente admin da plataforma.
        /// Remove a permissao do catalogo; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteAvailablePermission/{idAvailablePermission}")]
        public async Task<IActionResult> DeleteAvailablePermission(long idAvailablePermission)
        {
            var ret = await _app.Delete(idAvailablePermission);
            return Ok(ret);
        }
    }
}
