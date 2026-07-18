using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginAccountController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ILoginAccountApp _app;
        private readonly IAuthApp _authApp;

        public LoginAccountController(IConfiguration configuration, ILoginAccountApp app, IAuthApp authApp)
        {
            this._configuration = configuration;
            this._app = app;
            this._authApp = authApp;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO login)
        {
            var ret = await _authApp.Login(login);
            
            return Ok(ret);
        }
        [HttpGet("GetLoginAccountById")]
        public async Task<IActionResult> GetLoginAccountById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllLoginAccounts")]
        public async Task<IActionResult> GetAllLoginAccounts()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreateLoginAccount")]
        public async Task<IActionResult> CreateLoginAccount(CreateLoginAccountDTO entity)
        {
            // CreateLoginAccount DTO exists as CreateLoginAccount in Domain.DataTransferObject
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateLoginAccount")]
        public async Task<IActionResult> UpdateLoginAccount(UpdateLoginAccountDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteLoginAccount/{idLoginAccount}")]
        public async Task<IActionResult> DeleteLoginAccount(long idLoginAccount)
        {
            var ret = await _app.Delete(idLoginAccount);
            return Ok(ret);
        }
    }
}
