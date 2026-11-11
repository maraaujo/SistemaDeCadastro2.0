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

        /// <summary>
        /// Recebe a camada de aplicacao de tipos sanguineos por injecao.
        /// </summary>
        public BloodTypeController(IBloodTypeApp bloodTypeApp)
        {
            _bloodTypeApp = bloodTypeApp;
        }

        /// <summary>
        /// GET api/v1/BloodType - controller sem [Authorize], entao nao exige login.
        /// Retorna ApiResponse com a lista de tipos sanguineos em Data.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bloodTypeApp.GetAllBloodIllness();
            return Ok(result);
        }

        /// <summary>
        /// GET api/v1/BloodType/{id} - nao exige login.
        /// Retorna uma lista com o tipo sanguineo do id informado. Como a App devolve uma lista
        /// (nunca null), o NotFound na pratica nao ocorre: id inexistente gera lista vazia.
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _bloodTypeApp.GetBloodTypeById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

      

    }
}
