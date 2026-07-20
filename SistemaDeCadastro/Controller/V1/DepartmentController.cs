using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IDepartmentApp _departmentApp;

        public DepartmentController(IConfiguration configuration, IDepartmentApp departmentApp)
        {
            this._configuration = configuration;
            this._departmentApp = departmentApp;
        }

        [HttpGet("GetDepartmentById")]
        public async Task<IActionResult> GetDepartmentById(long id)
        {
            var department = await _departmentApp.GetById(id);
            return Ok(department);
        }

        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentApp.GetAll();
            return Ok(departments);
        }

        [HttpPost("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDTO department)
        {
            var ret = await _departmentApp.Create(department);
            return Ok(ret);
        }

        [HttpPut("UpdateDepartment")]
        public async Task<IActionResult> UpdateDepartment(UpdateDepartmentDTO department)
        {
            var ret = await _departmentApp.Update(department);
            return Ok(ret);
        }

        [HttpGet("DeleteDepartment/{idDepartment}")]
        public async Task<IActionResult> DeleteDepartment(long idDepartment)
        {
            var ret = await _departmentApp.Delete(idDepartment);
            return Ok(ret);
        }
    }
}
