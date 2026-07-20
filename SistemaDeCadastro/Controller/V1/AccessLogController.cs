using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessLogController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IAccessLogApp _app;

        public AccessLogController(IConfiguration configuration, IAccessLogApp app)
        {
            this._configuration = configuration;
            this._app = app;
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

        [HttpPost("CreateAccessLog")]
        public async Task<IActionResult> CreateAccessLog(CreateAccessLogDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateAccessLog")]
        public async Task<IActionResult> UpdateAccessLog(UpdateAccessLogDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteAccessLog/{idAccessLog}")]
        public async Task<IActionResult> DeleteAccessLog(long idAccessLog)
        {
            var ret = await _app.Delete(idAccessLog);
            return Ok(ret);
        }
    }
}
