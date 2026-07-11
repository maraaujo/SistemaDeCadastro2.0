using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientIllnessController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientIllnessApp _app;

        public PatientIllnessController(IConfiguration configuration, IPatientIllnessApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetPatientIllnessById")]
        public async Task<IActionResult> GetPatientIllnessById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllPatientIllness")]
        public async Task<IActionResult> GetAllPatientIllness()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreatePatientIllness")]
        public async Task<IActionResult> CreatePatientIllness(PatientIllness entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdatePatientIllness")]
        public async Task<IActionResult> UpdatePatientIllness(PatientIllness entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeletePatientIllness/{idPatientIllness}")]
        public async Task<IActionResult> DeletePatientIllness(long idPatientIllness)
        {
            var ret = await _app.Delete(idPatientIllness);
            return Ok(ret);
        }
    }
}
