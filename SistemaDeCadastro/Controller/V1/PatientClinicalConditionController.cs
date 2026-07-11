using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientClinicalConditionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientClinicalConditionApp _app;

        public PatientClinicalConditionController(IConfiguration configuration, IPatientClinicalConditionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetPatientClinicalConditionById")]
        public async Task<IActionResult> GetPatientClinicalConditionById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllPatientClinicalConditions")]
        public async Task<IActionResult> GetAllPatientClinicalConditions()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreatePatientClinicalCondition")]
        public async Task<IActionResult> CreatePatientClinicalCondition(PatientClinicalCondition entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdatePatientClinicalCondition")]
        public async Task<IActionResult> UpdatePatientClinicalCondition(PatientClinicalCondition entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeletePatientClinicalCondition/{idPatientClinicalCondition}")]
        public async Task<IActionResult> DeletePatientClinicalCondition(long idPatientClinicalCondition)
        {
            var ret = await _app.Delete(idPatientClinicalCondition);
            return Ok(ret);
        }
    }
}
