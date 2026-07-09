using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsibleController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IResponsibleApp _responsibleApp;

        public ResponsibleController(IConfiguration configuration, IResponsibleApp responsibleApp)
        {
            this._configuration = configuration;
            this._responsibleApp = responsibleApp;
        }

        [HttpGet("GetResponsibleById")]
        public async Task<IActionResult> GetResponsibleById(long id)
        {
            var responsible = await _responsibleApp.GetById(id);
            return Ok(responsible);
        }

        [HttpGet("GetAllResponsibles")]
        public async Task<IActionResult> GetAllResponsibles()
        {
            var responsibles = await _responsibleApp.GetAll();
            return Ok(responsibles);
        }

        [HttpPost("CreateResponsible")]
        public async Task<IActionResult> CreateResponsible(Responsible responsible)
        {
            var ret = await _responsibleApp.Create(responsible);
            return Ok(ret);
        }

        [HttpPut("UpdateResponsible")]
        public async Task<IActionResult> UpdateResponsible(Responsible responsible)
        {
            var ret = await _responsibleApp.Update(responsible);
            return Ok(ret);
        }

        [HttpGet("DeleteResponsible/{idResponsible}")]
        public async Task<IActionResult> DeleteResponsible(long idResponsible)
        {
            var ret = await _responsibleApp.Delete(idResponsible);
            return Ok(ret);
        }
    }
}
