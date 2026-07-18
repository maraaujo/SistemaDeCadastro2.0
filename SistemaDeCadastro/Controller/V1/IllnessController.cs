using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
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
            // TODO: Ajustar IIllnessApp e IllnessApp para receber CreateIllnessDTO
            var ret = await _app.CreateIllness(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateIllness")]
        public async Task<IActionResult> UpdateIllness(UpdateIllnessDTO entity)
        {
            // TODO: Ajustar IIllnessApp e IllnessApp para receber UpdateIllnessDTO
            var ret = await _app.UpdateIllness(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteIllness/{idIllness}")]
        public async Task<IActionResult> DeleteIllness(long idIllness)
        {
            var ret = await _app.DeleteIllness(idIllness);
            return Ok(ret);
        }
    }
}
