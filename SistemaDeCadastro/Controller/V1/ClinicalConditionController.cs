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

        public ClinicalConditionController(IConfiguration configuration, IClinicalConditionApp clinicalConditionApp)
        {
            this._configuration = configuration;
            this._clinicalConditionApp = clinicalConditionApp;
        }

        [HttpGet("GetClinicalConditionById")]
        public async Task<IActionResult> GetClinicalConditionById(long id)
        {
            var item = await _clinicalConditionApp.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllClinicalConditions")]
        public async Task<IActionResult> GetAllClinicalConditions()
        {
            var items = await _clinicalConditionApp.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateClinicalCondition")]
        public async Task<IActionResult> CreateClinicalCondition(CreateClinicalConditionDTO clinicalCondition)
        {
            var ret = await _clinicalConditionApp.Create(clinicalCondition);
            return Ok(ret);
        }

        [HttpPut("UpdateClinicalCondition")]
        public async Task<IActionResult> UpdateClinicalCondition(UpdateClinicalConditionDTO clinicalCondition)
        {
            var ret = await _clinicalConditionApp.Update(clinicalCondition);
            return Ok(ret);
        }

        [HttpGet("DeleteClinicalCondition/{idClinicalCondition}")]
        public async Task<IActionResult> DeleteClinicalCondition(long idClinicalCondition)
        {
            var ret = await _clinicalConditionApp.Delete(idClinicalCondition);
            return Ok(ret);
        }
        [HttpPost("GetClinicalConditionByFilter")]
        public async Task<IActionResult> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter)
        {
            var ret = await _clinicalConditionApp.GetClinicalConditionByFilter(filter);
            return Ok(ret);
        }
    }
}
