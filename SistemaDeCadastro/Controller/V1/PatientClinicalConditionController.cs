using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Filters;
namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientClinicalConditionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPatientClinicalConditionApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de condicoes clinicas do acolhido).
        /// </summary>
        public PatientClinicalConditionController(IConfiguration configuration, IPatientClinicalConditionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/PatientClinicalCondition/GetPatientClinicalConditionById?id= - qualquer usuario autenticado.
        /// Retorna o vinculo acolhido + condicao clinica pelo id, ou null.
        /// </summary>
        [HttpGet("GetPatientClinicalConditionById")]
        public async Task<IActionResult> GetPatientClinicalConditionById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }
        /// <summary>
        /// GET api/PatientClinicalCondition/GetPatientClinicalConditionByPatientId/{id} - usuario autenticado.
        /// Retorna as condicoes clinicas do acolhido informado (id = id do acolhido).
        /// </summary>
        [HttpGet("GetPatientClinicalConditionByPatientId/{id}")]
        public async Task<IActionResult> GetPatientClinicalConditionByPatientId(long id)
        {
            var item = await _app.GetPatientClinicalConditionByPatientId(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/PatientClinicalCondition/GetAllPatientClinicalConditions - qualquer usuario autenticado.
        /// Retorna todos os vinculos acolhido + condicao clinica visiveis para a instituicao do usuario.
        /// </summary>
        [HttpGet("GetAllPatientClinicalConditions")]
        public async Task<IActionResult> GetAllPatientClinicalConditions()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/PatientClinicalCondition/CreatePatientClinicalCondition - qualquer usuario autenticado.
        /// Associa uma condicao clinica a um acolhido; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreatePatientClinicalCondition")]
        public async Task<IActionResult> CreatePatientClinicalCondition(CreatePatientClinicalConditionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/PatientClinicalCondition/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o vinculo recebendo a propria entidade PatientClinicalCondition no corpo (nao um DTO);
        /// o id usado e o do corpo. Retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePatientClinicalCondition(PatientClinicalCondition entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/PatientClinicalCondition/DeletePatientClinicalCondition/{id} - qualquer usuario autenticado.
        /// Remove a condicao clinica do acolhido; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpDelete("DeletePatientClinicalCondition/{idPatientClinicalCondition}")]
        public async Task<IActionResult> DeletePatientClinicalCondition(long idPatientClinicalCondition)
        {
            var ret = await _app.Delete(idPatientClinicalCondition);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/PatientClinicalCondition/GetPatientClinicalConditionByFilter - usuario autenticado.
        /// Retorna a lista paginada de condicoes clinicas de acolhidos que atendem ao filtro do corpo.
        /// </summary>
        [HttpPost("GetPatientClinicalConditionByFilter")]
        public async Task<IActionResult> GetPatientClinicalConditionByFilter(PatientClinicalConditionFilterDTO filter)
        {
            var ret = await _app.GetPatientClinicalConditionByFilter(filter);
            return Ok(ret);
        }
    }
}
