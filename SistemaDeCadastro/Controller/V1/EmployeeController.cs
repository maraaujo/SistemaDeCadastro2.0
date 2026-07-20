using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IEmployeeApp _employeeApp;

        public EmployeeController(IConfiguration configuration, IEmployeeApp employeeApp)
        {
            this._configuration = configuration;
            this._employeeApp = employeeApp;
        }

        [HttpGet("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById(long id)
        {
            var employee = await _employeeApp.GetById(id);
            return Ok(employee);
        }

        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeApp.GetAll();
            return Ok(employees);
        }

        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDTO employee)
        {
             var ret = await _employeeApp.Create(employee);
            return Ok(ret);
        }

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeDTO employee)
        {
            var ret = await _employeeApp.Update(employee);
            return Ok(ret);
        }

        [HttpGet("DeleteEmployee/{idEmployee}")]
        public async Task<IActionResult> DeleteEmployee(long idEmployee)
        {
            var ret = await _employeeApp.Delete(idEmployee);
            return Ok(ret);
        }
    }
}
