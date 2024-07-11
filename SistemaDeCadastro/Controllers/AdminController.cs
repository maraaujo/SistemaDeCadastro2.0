using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.Controllers
{    
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IIdosoApp idosoApp;
        private IConfiguration config;
        private readonly IAdminApp adminApp;
        private IMapper mapper;
        private UserManager<Usuario> userManager;

        public AdminController(IConfiguration _config, IIdosoApp _idosoApp, IAdminApp _adminApp, IMapper _mapper)
        {
            this.config = _config;
            this.idosoApp = _idosoApp;
            this.adminApp = _adminApp;
            this.mapper = _mapper;
        }

        [HttpPost("CadastrarFuncionario")]
        public async Task<IActionResult> CadastrarUsuario(UsuarioDTO usuario)
        {
           Usuario createUsuario = mapper.Map<Usuario>(usuario);
           var resultado = await userManager.CreateAsync(createUsuario, usuario.Password);
            if (resultado.Succeeded)
            {
                return Ok(resultado + "Usuario Cadastrado com Sucesso!");
            }
            else
            {
                throw new Exception("falha ao cadastrar um usuario");
            }
        }

        [HttpGet("GetIdoso")]
        public async Task<IActionResult> GetIdoso(IdosoFilterDTO filter)
        {
            ApiResponseDTO ret = new();
            try
            {
                ret.Data = await this.adminApp.GetIdoso(filter);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return Ok(ret);
        } 
        [HttpGet("GetAllIdoso")]
        public async Task<IActionResult> GetAllIdoso()
        {
            ApiResponseDTO ret = new();
            try
            {
                ret.Data = await this.adminApp.GetAllIdoso();
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return Ok(ret);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(IdosoDTO idosoDTO)
        {
            var response = await this.adminApp.Create(idosoDTO);

            if (response.Success == false)
                return BadRequest(response.ErrorMessage);
            else
                return Ok(response);
        }
        [HttpPut("Update")]
        public async Task <IActionResult> Update(Idoso idoso)
        {
             ApiResponseDTO response = new ApiResponseDTO();
            try
            {
               await this.adminApp.Update(idoso);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;   
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);

        }
        [HttpGet("GetIdosobyId")]
        public async Task<IActionResult> GetIdosoById(int id)
        {
            ApiResponseDTO ret = new();
            try
            {
                ret.Data = await this.adminApp.GetIdosoById(id);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return Ok(ret);
        }  
        [HttpGet("GetElderlyMedicine")]
        public async Task<IActionResult> GetElderlyMedicine(int id)
        {
            ApiResponseDTO ret = new();
            try
            {
                ret.Data = await this.adminApp.GetElderlyMedicine(id);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return Ok(ret);
        }

        [HttpGet("ValidateFuncionario")]
        public async Task<IActionResult> ValidateFuncionario(string email, string senha)
        {
            ApiResponseDTO ret = new();
            try
            {
                ret.Data = await this.adminApp.ValidateFuncionario(email,senha);
                ret.Success = true;
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
