using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Filters;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IEmployeeApp _employeeApp;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de funcionarios).
        /// </summary>
        public EmployeeController(IConfiguration configuration, IEmployeeApp employeeApp)
        {
            this._configuration = configuration;
            this._employeeApp = employeeApp;
        }

        /// <summary>
        /// GET api/Employee/GetEmployeeById/{id} - qualquer usuario autenticado.
        /// Retorna o funcionario pelo id (null se nao existir ou for de outra instituicao).
        /// </summary>
        [HttpGet("GetEmployeeById/{id}")]
        public async Task<IActionResult> GetEmployeeById(long id)
        {
            var employee = await _employeeApp.GetById(id);
            return Ok(employee);
        }

        /// <summary>
        /// GET api/Employee/GetAllEmployees - qualquer usuario autenticado.
        /// Retorna todos os funcionarios visiveis para a instituicao do usuario logado.
        /// </summary>
        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeApp.GetAll();
            return Ok(employees);
        }

        /// <summary>
        /// POST api/Employee/CreateEmployee - qualquer usuario autenticado.
        /// Cadastra um novo funcionario; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDTO employee)
        {
             var ret = await _employeeApp.Create(employee);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Employee/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o funcionario com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeDTO employee)
        {
            var ret = await _employeeApp.Update(employee);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Employee/DeleteEmployee/{idEmployee} - qualquer usuario autenticado.
        /// Exclui o funcionario informado; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpDelete("DeleteEmployee/{idEmployee}")]
        public async Task<IActionResult> DeleteEmployee(long idEmployee)
        {
            var ret = await _employeeApp.Delete(idEmployee);
            return Ok(ret);
        }
        
        /// <summary>
        /// POST api/Employee/GetEmployeeByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de funcionarios que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetEmployeeByFilter")]
        public async Task<IActionResult> GetEmployeeByFilter(EmployeeFilterDTO filter)
        {
            var employees = await _employeeApp.GetEmployeeByFilter(filter);
            return Ok(employees);
        }

    }
}
