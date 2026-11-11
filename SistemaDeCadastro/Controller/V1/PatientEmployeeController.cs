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
    public class PatientEmployeeController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientEmployeeApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao do vinculo acolhido-funcionario).
        /// </summary>
        public PatientEmployeeController(IConfiguration configuration, IPatientEmployeeApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/PatientEmployee/GetPatientEmployeeById?id= - qualquer usuario autenticado.
        /// Retorna o vinculo acolhido-funcionario pelo id, ou null.
        /// </summary>
        [HttpGet("GetPatientEmployeeById")]
        public async Task<IActionResult> GetPatientEmployeeById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/PatientEmployee/GetAllPatientEmployees - qualquer usuario autenticado.
        /// Retorna todos os vinculos acolhido-funcionario visiveis para a instituicao do usuario.
        /// </summary>
        [HttpGet("GetAllPatientEmployees")]
        public async Task<IActionResult> GetAllPatientEmployees()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/PatientEmployee/CreatePatientEmployee - qualquer usuario autenticado.
        /// Vincula um funcionario a um acolhido; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreatePatientEmployee")]
        public async Task<IActionResult> CreatePatientEmployee(CreatePatientEmployeeDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/PatientEmployee/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o vinculo com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePatientEmployee(UpdatePatientEmployeeDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/PatientEmployee/DeletePatientEmployee/{idPatientEmployee} - qualquer usuario autenticado.
        /// Remove o vinculo acolhido-funcionario; retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeletePatientEmployee/{idPatientEmployee}")]
        public async Task<IActionResult> DeletePatientEmployee(long idPatientEmployee)
        {
            var ret = await _app.Delete(idPatientEmployee);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/PatientEmployee/GetPagedPatientEmployeeByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de vinculos acolhido-funcionario que atendem ao filtro do corpo.
        /// </summary>
        [HttpPost("GetPagedPatientEmployeeByFilter")]
        public async Task<IActionResult> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter)
        {
            var ret = await _app.GetPagedPatientEmployeeByFilter(filter);
            return Ok(ret);
        }
    }
}
