using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
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
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllIllness")]
        public async Task<IActionResult> GetAllIllness()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateIllness")]
        public async Task<IActionResult> CreateIllness(Illness entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateIllness")]
        public async Task<IActionResult> UpdateIllness(Illness entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteIllness/{idIllness}")]
        public async Task<IActionResult> DeleteIllness(long idIllness)
        {
            var ret = await _app.Delete(idIllness);
            return Ok(ret);
        }
    }
}
