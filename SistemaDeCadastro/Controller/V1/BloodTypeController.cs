using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BloodTypeController : ControllerBase
    {
        private readonly IBloodTypeApp _bloodTypeApp;

        public BloodTypeController(IBloodTypeApp bloodTypeApp)
        {
            _bloodTypeApp = bloodTypeApp;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bloodTypeApp.GetAllBloodIllness();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _bloodTypeApp.GetBloodTypeById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

      

    }
}
