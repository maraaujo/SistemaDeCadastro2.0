
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

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
        [HttpGet("GetByAny")]
        public async Task<IActionResult> GetPatientByAny(string patient)
        {
            await _patientApp.GetPatientByAny(patient);
            return Ok();
        }
        [HttpPost("subscriptions")]
        public async Task<IActionResult> GetSubscriptions(PatientFilterDTO filter)
        {
            ApiResponse response = new ApiResponse();

            try
            {
                response.Data = await this._patientApp.FilterPatient(filter);
                response.Success = true;
            }
            catch (Exception err)
            {
                response.Success = false;
                response.Message = err.Message;
            }

            return Ok(response);
        }
        [HttpPost("Createpatient")]
        public async Task<IActionResult> CreatePatient(PatientDTO patient)
        {
            var ret = await _patientApp.CreatePatient(patient);
            return Ok(ret);
        }
        [HttpPut("Updatpatient")]
        public async Task<IActionResult> Updatepatient(PatientDTO patient)
        {
            var ret = await _patientApp.UpdatePatient(patient);
            return Ok(ret);
        }

        [HttpDelete("DeletePatient")]
        public async Task<IActionResult> DeletePatient(PatientDTO patient)
        {
            var ret = _patientApp.DeletePatient(patient);
            return Ok(ret);
        }
        
    }
}
