using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPermissionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IUserPermissionApp _app;

        public UserPermissionController(IConfiguration configuration, IUserPermissionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetUserPermissionById")]
        public async Task<IActionResult> GetUserPermissionById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllUserPermission")]
        public async Task<IActionResult> GetAllUserPermission()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateUserPermission")]
        public async Task<IActionResult> CreateUserPermission(UserPermission entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateUserPermission")]
        public async Task<IActionResult> UpdateUserPermission(UserPermission entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteUserPermission/{idUserPermission}")]
        public async Task<IActionResult> DeleteUserPermission(long idUserPermission)
        {
            var ret = await _app.Delete(idUserPermission);
            return Ok(ret);
        }
    }
}
