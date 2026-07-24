using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IllnessController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IIllnessApp _app;

        public IllnessController(IConfiguration configuration, IIllnessApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetIllnessById")]
        public async Task<IActionResult> GetIllnessById(long id)
        {
            var item = await _app.GetIllnessById(id);
            return Ok(item);
        }

        [HttpGet("GetAllIllness")]
        public async Task<IActionResult> GetAllIllness()
        {
            var items = await _app.GetAllIllness();
            return Ok(items);
        }

        [HttpPost("CreateIllness")]
        public async Task<IActionResult> CreateIllness(CreateIllnessDTO entity)
        {
            var ret = await _app.CreateIllness(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateIllness")]
        public async Task<IActionResult> UpdateIllness(UpdateIllnessDTO entity)
        {
            var ret = await _app.UpdateIllness(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteIllness/{idIllness}")]
        public async Task<IActionResult> DeleteIllness(IllnessDTO idIllness)
        {
            var ret = await _app.DeleteIllness(idIllness);
            return Ok(ret);
        }
        [HttpPost("GetIllnessByFilter")]
        public async Task<IActionResult> GetIllnessByFilter(IllnessFilterDTO filter)
        {
            var ret = await _app.GetIllnessByFilter(filter);
            return Ok(ret);
        }
    }
}
