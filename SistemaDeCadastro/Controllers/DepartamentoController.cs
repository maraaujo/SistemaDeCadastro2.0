using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartamentoController : ControllerBase
    { 
        private readonly IDepartamentoApp departamentoApp;
        private IConfiguration configuration;

        public DepartamentoController(IDepartamentoApp _departamentoApp, IConfiguration _configuration)
        {
            this.configuration = configuration;
            this.departamentoApp = _departamentoApp;
        }

        [HttpPost("CadastrarDepartamento")]
        public async Task<IActionResult> Cadastrar(DepartamentoDTO departamentodto)
        {
            ApiResponseDTO ret = new ApiResponseDTO();
            try
            {
                await departamentoApp.CreateDepartamento(departamentodto);
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }

            return Ok(ret);
        }
        [HttpGet("ProcurarPorId")]
        public async Task<IActionResult> GetById(int id)
        {
            ApiResponseDTO ret = new ApiResponseDTO();
            try
            {
                ret.Data = await departamentoApp.GetDepartamentoById(id);

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return Ok(ret);

        }
    }
}
