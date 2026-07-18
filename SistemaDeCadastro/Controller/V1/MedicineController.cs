using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IMedicineApp _app;

        public MedicineController(IConfiguration configuration, IMedicineApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetMedicineById")]
        public async Task<IActionResult> GetMedicineById(long id)
        {
            var item = await _app.GetMedicineId(id);
            return Ok(item);
        }

        [HttpGet("GetAllMedicines")]
        public async Task<IActionResult> GetAllMedicines()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateMedicine")]
        public async Task<IActionResult> CreateMedicine(CreateMedicineDTO entity)
        {
            // TODO: Ajustar IMedicineApp e MedicineApp para receber CreateMedicineDTO
            var ret = await _app.CreateMedicine(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateMedicine")]
        public async Task<IActionResult> UpdateMedicine(MedicineDTO entity)
        {
            var ret = await _app.UpdateMedicine(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteMedicine/{idMedicine}")]
        public async Task<IActionResult> DeleteMedicine(long idMedicine)
        {
            var dto = new MedicineDTO { Id = idMedicine };
            var ret = await _app.DeleteMedicine(dto);
            return Ok(ret);
        }
    }
}
