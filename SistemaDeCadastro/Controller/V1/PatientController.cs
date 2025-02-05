
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpGet("GetPatientFilter")]
        public async Task<IActionResult> GetPatientFilter(PatientFilterDTO filter)
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
        [HttpGet("GetPatientDetails")]
        public async Task<IActionResult> DetailsPatient()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                response.Data = await this._patientApp.DetailsPatient();
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
        public async Task<IActionResult> CreatePatient(CreatepatientDTO patient)
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

        [HttpGet("DeletePatient/{idPatient}")]
        public async Task<IActionResult> DeletePatient(long idPatient)
        {
            var ret = await _patientApp.DeletePatient(idPatient);
            return Ok(ret);

            // ctrl + M + F = C M F = CADE MEU FILE
        }
        [HttpGet("GetMedicinesToMinister")]
       public async Task<IActionResult> GetMedicinesToMinister()
        {
            var ret = await _patientApp.GetMedicinesToMinister();   
            return Ok(ret);
        }
    }
}
