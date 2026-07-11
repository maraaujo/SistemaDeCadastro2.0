using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailablePermissionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IAvailablePermissionApp _app;

        public AvailablePermissionController(IConfiguration configuration, IAvailablePermissionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetAvailablePermissionById")]
        public async Task<IActionResult> GetAvailablePermissionById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllAvailablePermission")]
        public async Task<IActionResult> GetAllAvailablePermission()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateAvailablePermission")]
        public async Task<IActionResult> CreateAvailablePermission(AvailablePermission entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateAvailablePermission")]
        public async Task<IActionResult> UpdateAvailablePermission(AvailablePermission entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteAvailablePermission/{idAvailablePermission}")]
        public async Task<IActionResult> DeleteAvailablePermission(long idAvailablePermission)
        {
            var ret = await _app.Delete(idAvailablePermission);
            return Ok(ret);
        }
    }
}
