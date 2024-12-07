
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PatientController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientApp _patientApp;

        public PatientController(IConfiguration configuration, IPatientApp patientApp)
        {
            this._configuration = configuration;
            this._patientApp = patientApp;
        }

        [HttpGet("GetPatientById")]
        public async Task<IActionResult> GetPatientById(long id)
        {
            var patient = await _patientApp.GetPatientById(id);
            return Ok(patient);
        }

    }
}
