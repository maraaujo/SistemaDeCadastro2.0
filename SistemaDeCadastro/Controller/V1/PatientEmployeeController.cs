using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientEmployeeController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientEmployeeApp _app;

        public PatientEmployeeController(IConfiguration configuration, IPatientEmployeeApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetPatientEmployeeById")]
        public async Task<IActionResult> GetPatientEmployeeById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllPatientEmployees")]
        public async Task<IActionResult> GetAllPatientEmployees()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreatePatientEmployee")]
        public async Task<IActionResult> CreatePatientEmployee(PatientEmployee entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdatePatientEmployee")]
        public async Task<IActionResult> UpdatePatientEmployee(PatientEmployee entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeletePatientEmployee/{idPatientEmployee}")]
        public async Task<IActionResult> DeletePatientEmployee(long idPatientEmployee)
        {
            var ret = await _app.Delete(idPatientEmployee);
            return Ok(ret);
        }
    }
}
