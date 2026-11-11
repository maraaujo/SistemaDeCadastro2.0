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
    public class ClinicalConditionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IClinicalConditionApp _clinicalConditionApp;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de condicoes clinicas).
        /// </summary>
        public ClinicalConditionController(IConfiguration configuration, IClinicalConditionApp clinicalConditionApp)
        {
            this._configuration = configuration;
            this._clinicalConditionApp = clinicalConditionApp;
        }

        /// <summary>
        /// GET api/ClinicalCondition/GetClinicalConditionById?id= - qualquer usuario autenticado.
        /// Retorna uma condicao clinica do catalogo (null se nao existir).
        /// </summary>
        [HttpGet("GetClinicalConditionById")]
        public async Task<IActionResult> GetClinicalConditionById(long id)
        {
            var item = await _clinicalConditionApp.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/ClinicalCondition/GetAllClinicalConditions - qualquer usuario autenticado.
        /// Retorna todo o catalogo de condicoes clinicas (compartilhado entre as instituicoes).
        /// </summary>
        [HttpGet("GetAllClinicalConditions")]
        public async Task<IActionResult> GetAllClinicalConditions()
        {
            var items = await _clinicalConditionApp.GetAll();
            return Ok(items);
        }

        // Catalogo unico compartilhado por todas as instituicoes: so o admin da plataforma altera.
        /// <summary>
        /// POST api/ClinicalCondition/CreateClinicalCondition - somente admin da plataforma (policy "SaasAdmin").
        /// Cadastra uma nova condicao clinica no catalogo; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateClinicalCondition")]
        public async Task<IActionResult> CreateClinicalCondition(CreateClinicalConditionDTO clinicalCondition)
        {
            var ret = await _clinicalConditionApp.Create(clinicalCondition);
            return Ok(ret);
        }

        // Catalogo unico compartilhado por todas as instituicoes: so o admin da plataforma altera.
        /// <summary>
        /// PUT api/ClinicalCondition/Update/{id} - somente admin da plataforma (policy "SaasAdmin").
        /// Atualiza a condicao clinica com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateClinicalCondition(UpdateClinicalConditionDTO clinicalCondition)
        {
            var ret = await _clinicalConditionApp.Update(clinicalCondition);
            return Ok(ret);
        }

        // Catalogo unico compartilhado por todas as instituicoes: so o admin da plataforma altera.
        /// <summary>
        /// DELETE api/ClinicalCondition/DeleteClinicalCondition/{idClinicalCondition} - somente admin da plataforma.
        /// Remove a condicao clinica do catalogo; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteClinicalCondition/{idClinicalCondition}")]
        public async Task<IActionResult> DeleteClinicalCondition(long idClinicalCondition)
        {
            var ret = await _clinicalConditionApp.Delete(idClinicalCondition);
            return Ok(ret);
        }
        /// <summary>
        /// POST api/ClinicalCondition/GetClinicalConditionByFilter - qualquer usuario autenticado.
        /// Retorna a lista paginada de condicoes clinicas que atendem ao filtro enviado no corpo.
        /// </summary>
        [HttpPost("GetClinicalConditionByFilter")]
        public async Task<IActionResult> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter)
        {
            var ret = await _clinicalConditionApp.GetClinicalConditionByFilter(filter);
            return Ok(ret);
        }
    }
}
