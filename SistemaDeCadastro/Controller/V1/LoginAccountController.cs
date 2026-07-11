using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginAccountController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ILoginAccountApp _app;

        public LoginAccountController(IConfiguration configuration, ILoginAccountApp app)
        {
            this._configuration = configuration;
            this._app = app;
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
        public async Task<IActionResult> CreateLoginAccount(LoginAccount entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateLoginAccount")]
        public async Task<IActionResult> UpdateLoginAccount(LoginAccount entity)
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
