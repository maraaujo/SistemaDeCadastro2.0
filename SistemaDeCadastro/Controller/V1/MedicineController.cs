using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using Microsoft.AspNetCore.Authorization;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            var ret = await _app.CreateMedicine(entity);
            return Ok(ret);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateMedicine(long id, UpdateMedicineDTO entity)
        {
            // Garante o Id vindo da rota (o front pode mandar so na URL, nao no corpo).
            // Sem isso, entity.Id ficava 0 e o update "nao encontrava" o medicamento,
            // retornando 200 mas sem salvar nada.
            if (entity.Id == 0)
                entity.Id = id;

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
        [HttpPost("GetMedicineByFilter")]
        public async Task<IActionResult> GetMedicineByFilter(MedicineFilterDTO filter)
        {
            var ret = await _app.GetMedicineByFilter(filter);
            return Ok(ret);
        }
    }
}
