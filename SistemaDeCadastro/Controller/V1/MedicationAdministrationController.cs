using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicationAdministrationController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IMedicationAdministrationApp _app;

        public MedicationAdministrationController(IConfiguration configuration, IMedicationAdministrationApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetMedicationAdministrationById")]
        public async Task<IActionResult> GetMedicationAdministrationById    (long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        //[HttpGet("GetAllMedicationAdministrations")]
        //public async Task<IActionResult> GetAllMedicationAdministrations()
        //{
        //    var items = await _app.GetAll();
        //    return Ok(items);
        //}

        [HttpPost("CreateMedicationAdministration")]
        public async Task<IActionResult> CreateMedicationAdministration(CreateMedicationAdministrationDTO entity)
        {
            // TODO: Ajustar IMedicationAdministrationApp e MedicationAdministrationApp para receber CreateMedicationAdministrationDTO
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateMedicationAdministration")]
        public async Task<IActionResult> UpdateMedicationAdministration(UpdateMedicationAdministrationDTO entity)
        {
            // TODO: Ajustar IMedicationAdministrationApp e MedicationAdministrationApp para receber UpdateMedicationAdministrationDTO
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteMedicationAdministration/{idMedicationAdministration}")]
        public async Task<IActionResult> DeleteMedicationAdministration(MedicationAdministration idMedicationAdministration)
        {
            var ret = await _app.Delete(idMedicationAdministration);
            return Ok(ret);
        }
        [HttpPost("GetPagedMedicationAdministrationByFilter")]
        public async Task<IActionResult> GetPagedMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter)
        {
            var pagedMedicationAdministrations = await _app.GetMedicationAdministrationByFilter(filter);
            return Ok(pagedMedicationAdministrations);
        }
    }
}
