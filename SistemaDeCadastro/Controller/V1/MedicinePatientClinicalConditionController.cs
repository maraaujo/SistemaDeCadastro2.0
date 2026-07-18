using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinePatientClinicalConditionController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IMedicinePatientClinicalConditionApp _app;

        public MedicinePatientClinicalConditionController(IConfiguration configuration, IMedicinePatientClinicalConditionApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateMedicinePatientClinicalConditionDTO entity)
        {
            // TODO: Ajustar IMedicinePatientClinicalConditionApp e MedicinePatientClinicalConditionApp para receber CreateMedicinePatientClinicalConditionDTO
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(UpdateMedicinePatientClinicalConditionDTO entity)
        {
            // TODO: Ajustar IMedicinePatientClinicalConditionApp e MedicinePatientClinicalConditionApp para receber UpdateMedicinePatientClinicalConditionDTO
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var ret = await _app.Delete(id);
            return Ok(ret);
        }

        [HttpGet("GetByPatientClinicalConditionId")]
        public async Task<IActionResult> GetByPatientClinicalConditionId(long patientClinicalConditionId)
        {
            var ret = await _app.GetByPatientClinicalConditionId(patientClinicalConditionId);
            return Ok(ret);
        }

        [HttpGet("GetMedicineReminders")]
        public async Task<IActionResult> GetMedicineReminders()
        {
            var ret = await _app.GetMedicineReminders();
            return Ok(ret);
        }
    }
}
