using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionApp _app;

        public InstitutionController(IInstitutionApp app)
        {
            _app = app;
        }

        [HttpGet("GetInstitutionById")]
        public async Task<IActionResult> GetInstitutionById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        [HttpGet("GetAllInstitutions")]
        public async Task<IActionResult> GetAllInstitutions()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        [HttpPost("CreateInstitution")]
        public async Task<IActionResult> CreateInstitution(CreateInstitutionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateInstitution")]
        public async Task<IActionResult> UpdateInstitution(UpdateInstitutionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteInstitution/{idInstitution}")]
        public async Task<IActionResult> DeleteInstitution(long idInstitution)
        {
            var ret = await _app.Delete(idInstitution);
            return Ok(ret);
        }

    }
}
