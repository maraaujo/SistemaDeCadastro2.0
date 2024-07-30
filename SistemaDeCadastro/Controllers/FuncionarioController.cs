using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioApp funcionarioApp;
        private IConfiguration config;

        public FuncionarioController(IConfiguration _config, IFuncionarioApp _funcionarioApp)
        {
            this.config = _config;
            this.funcionarioApp = _funcionarioApp;  
        }

        [HttpPost("CadastrarFuncionario")]

        public async Task<IActionResult> Cadastrar(FuncionarioDTO funcionariodto)
        {
            ApiResponseDTO ret = new ApiResponseDTO();
            try
            {
               await funcionarioApp.CreateFuncionario(funcionariodto);
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
              ret.Data = await funcionarioApp.GetFuncionairoById(id);
               
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
