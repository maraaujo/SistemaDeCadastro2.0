using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IDepartmentApp _departmentApp;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de setores).
        /// </summary>
        public DepartmentController(IConfiguration configuration, IDepartmentApp departmentApp)
        {
            this._configuration = configuration;
            this._departmentApp = departmentApp;
        }

        /// <summary>
        /// GET api/Department/GetDepartmentById?id= - qualquer usuario autenticado.
        /// Retorna o setor/departamento pelo id (null se nao existir ou for de outra instituicao).
        /// </summary>
        [HttpGet("GetDepartmentById")]
        public async Task<IActionResult> GetDepartmentById(long id)
        {
            var department = await _departmentApp.GetById(id);
            return Ok(department);
        }

        /// <summary>
        /// GET api/Department/GetAllDepartments - qualquer usuario autenticado.
        /// Retorna todos os setores visiveis para a instituicao do usuario logado.
        /// </summary>
        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentApp.GetAll();
            return Ok(departments);
        }

        /// <summary>
        /// POST api/Department/CreateDepartment - qualquer usuario autenticado.
        /// Cadastra um novo setor; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment(CreateDepartmentDTO department)
        {
            var ret = await _departmentApp.Create(department);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Department/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o setor com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateDepartment(UpdateDepartmentDTO department)
        {
            var ret = await _departmentApp.Update(department);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Department/DeleteDepartment/{idDepartment} - qualquer usuario autenticado.
        /// Exclui o setor informado; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpDelete("DeleteDepartment/{idDepartment}")]
        public async Task<IActionResult> DeleteDepartment(long idDepartment)
        {
            var ret = await _departmentApp.Delete(idDepartment);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/Department/GetDepartmentByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de setores que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetDepartmentByFilter")]
        public async Task<IActionResult> GetDepartmentByFilter(DepartmentFilterDTO filter)
        {
            var ret = await _departmentApp.GetDepartmentByFilter(filter);
            return Ok(ret);
        }
    }
}
