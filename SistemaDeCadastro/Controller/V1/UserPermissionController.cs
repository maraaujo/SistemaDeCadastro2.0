using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    // Gestao da plataforma: dados de todas as instituicoes, so o admin da plataforma.
    [Authorize(Policy = "SaasAdmin")]
    public class UserPermissionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IUserPermissionApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de permissoes de usuario).
        /// </summary>
        public UserPermissionController(IConfiguration configuration, IUserPermissionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/UserPermission/GetUserPermissionById?id= - somente admin da plataforma (policy da classe).
        /// Retorna a permissao atribuida a um usuario pelo id, ou null.
        /// </summary>
        [HttpGet("GetUserPermissionById")]
        public async Task<IActionResult> GetUserPermissionById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/UserPermission/GetAllUserPermission - somente admin da plataforma.
        /// Retorna todas as permissoes atribuidas a usuarios.
        /// </summary>
        [HttpGet("GetAllUserPermission")]
        public async Task<IActionResult> GetAllUserPermission()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/UserPermission/CreateUserPermission - somente admin da plataforma.
        /// Atribui uma permissao a um usuario; retorna ApiResponse.
        /// </summary>
        [HttpPost("CreateUserPermission")]
        public async Task<IActionResult> CreateUserPermission(CreateUserPermissionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/UserPermission/Update/{id} - somente admin da plataforma (o id usado e o do DTO).
        /// Atualiza a permissao do usuario; retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateUserPermission(UpdateUserPermissionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/UserPermission/DeleteUserPermission/{idUserPermission} - somente admin da plataforma.
        /// Remove a permissao do usuario; retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeleteUserPermission/{idUserPermission}")]
        public async Task<IActionResult> DeleteUserPermission(long idUserPermission)
        {
            var ret = await _app.Delete(idUserPermission);
            return Ok(ret);
        }
    }
}
