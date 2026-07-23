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
    public class CareServiceController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ICareServiceApp _app;

        public CareServiceController(IConfiguration configuration, ICareServiceApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetCareServiceById")]
        public async Task<IActionResult> GetCareServiceById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllCareServices")]
        public async Task<IActionResult> GetAllCareServices()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateCareService")]
        public async Task<IActionResult> CreateCareService(CreateCareServiceDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateCareService")]
        public async Task<IActionResult> UpdateCareService(UpdateCareServiceDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteCareService/{idCareService}")]
        public async Task<IActionResult> DeleteCareService(long idCareService)
        {
            var ret = await _app.Delete(idCareService);
            return Ok(ret);
        }
        [HttpPost("GetCareServiceByFilter")]
        public async Task<IActionResult> GetCareServiceByFilter(CareServiceFilterDTO filter)
        {
            var items = await _app.GetCareServiceByFilter(filter);
            return Ok(items);
        }
    }
}
