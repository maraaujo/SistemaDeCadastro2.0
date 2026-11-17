using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SistemaDeCadastro.APP.APP;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoginAccountController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ILoginAccountApp _app;
        private readonly IAuthApp _authApp;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao, App de contas de acesso e App de autenticacao).
        /// </summary>
        public LoginAccountController(IConfiguration configuration, ILoginAccountApp app, IAuthApp authApp)
        {
            this._configuration = configuration;
            this._app = app;
            this._authApp = authApp;
        }
        /// <summary>
        /// POST api/LoginAccount/Login - publico ([AllowAnonymous]), com limite de requisicoes (policy "login").
        /// Valida e-mail e senha (com bloqueio por excesso de tentativas) e, se der certo, devolve
        /// ApiResponse com os dados do usuario e o token JWT em Data. Sempre responde 200; o sucesso vem em Success.
        /// </summary>
        [HttpPost("Login")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login(LoginRequestDTO login)
        {
            var ret = await _authApp.Login(login);
            
            return Ok(ret);
        }
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request)
        {
            var ret = await _authApp.RequestPasswordReset(request);
            return Ok(ret);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO request)
        {
            var ret = await _authApp.ResetPassword(request);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/LoginAccount/GetLoginAccountById/{id} - qualquer usuario autenticado.
        /// Retorna a conta (sem hash de senha) se o usuario puder ve-la (quem administra a conta ou o proprio dono);
        /// caso contrario, ou se nao existir, retorna null.
        /// </summary>
        [HttpGet("GetLoginAccountById/{id}")]
        public async Task<IActionResult> GetLoginAccountById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/LoginAccount/GetAllLoginAccounts - qualquer usuario autenticado.
        /// Lista contas conforme o papel: admin da plataforma ve todas, admin de instituicao ve as da
        /// propria instituicao e os demais so a propria conta. Nunca devolve o hash da senha.
        /// </summary>
        [HttpGet("GetAllLoginAccounts")]
        public async Task<IActionResult> GetAllLoginAccounts()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/LoginAccount/CreateLoginAccount - usuario autenticado; a App so permite administradores.
        /// Cria uma conta de acesso (senha forte, e-mail unico, senha gravada com BCrypt). Admin de instituicao
        /// cria sempre na propria instituicao. Retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreateLoginAccount")]
        public async Task<IActionResult> CreateLoginAccount(CreateLoginAccountDTO entity)
        {
            // CreateLoginAccount DTO exists as CreateLoginAccount in Domain.DataTransferObject
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/LoginAccount/Update/{id} - qualquer usuario autenticado (o id usado e o do DTO).
        /// Atualiza nome/e-mail da conta; papel e status so mudam se o usuario logado administra a conta
        /// (o proprio dono, na tela de perfil, nao muda o papel). Retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateLoginAccount(UpdateLoginAccountDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/LoginAccount/DeleteLoginAccount/{idLoginAccount} - usuario autenticado.
        /// Exclui a conta se o usuario logado a administra (admin da plataforma ou admin da mesma instituicao);
        /// senao retorna "Conta nao encontrada.". Retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeleteLoginAccount/{idLoginAccount}")]
        public async Task<IActionResult> DeleteLoginAccount(long idLoginAccount)
        {
            var ret = await _app.Delete(idLoginAccount);
            return Ok(ret);
        }
    }
}
