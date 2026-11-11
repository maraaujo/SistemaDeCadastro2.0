using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionApp _app;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Recebe as dependencias por injecao (App de instituicoes e o servico de autorizacao usado para checar a policy "SaasAdmin").
        /// </summary>
        public InstitutionController(IInstitutionApp app, IAuthorizationService authorizationService)
        {
            _app = app;
            _authorizationService = authorizationService;
        }

        // Usuario de instituicao le apenas a propria instituicao (ex.: tela de perfil);
        // o admin da plataforma (policy SaasAdmin) le qualquer uma.
        /// <summary>
        /// GET api/Institution/GetInstitutionById/{id} - usuario autenticado.
        /// Retorna 403 (Forbid) se o usuario nao puder acessar essa instituicao; caso contrario
        /// devolve ApiResponse com a instituicao em Data (null se nao existir).
        /// </summary>
        [HttpGet("GetInstitutionById/{id}")]
        public async Task<IActionResult> GetInstitutionById(long id)
        {
            if (!await CanAccessInstitution(User, id, _authorizationService))
                return Forbid();

            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        /// <summary>
        /// Verifica se o usuario pode acessar a instituicao informada. Se o token tem a claim
        /// "institutionId" (ou "InstitutionId"), so libera quando for a mesma instituicao; sem essa
        /// claim, libera apenas se o usuario passar na policy "SaasAdmin" (admin da plataforma).
        /// Metodo internal static, reutilizado por outros controllers (ex.: assinaturas).
        /// </summary>
        internal static async Task<bool> CanAccessInstitution(
            System.Security.Claims.ClaimsPrincipal user, long institutionId, IAuthorizationService authorizationService)
        {
            var claim = user.FindFirst("institutionId")?.Value ?? user.FindFirst("InstitutionId")?.Value;
            if (long.TryParse(claim, out var ownInstitutionId))
                return ownInstitutionId == institutionId;

            return (await authorizationService.AuthorizeAsync(user, "SaasAdmin")).Succeeded;
        }

        /// <summary>
        /// GET api/Institution/GetAllInstitutions - somente admin da plataforma (policy "SaasAdmin").
        /// Retorna ApiResponse com todas as instituicoes cadastradas em Data.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("GetAllInstitutions")]
        public async Task<IActionResult> GetAllInstitutions()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        /// <summary>
        /// POST api/Institution/CreateInstitution - somente admin da plataforma (policy "SaasAdmin").
        /// Cadastra uma nova instituicao (valida o telefone e grava so os numeros); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateInstitution")]
        public async Task<IActionResult> CreateInstitution(CreateInstitutionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Institution/Update/{id} - somente admin da plataforma (policy "SaasAdmin").
        /// Atualiza a instituicao com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateInstitution(UpdateInstitutionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Institution/DeleteInstitution/{idInstitution} - somente admin da plataforma.
        /// Exclui a instituicao; retorna ApiResponse ("Instituicao nao encontrada." se nao existir).
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteInstitution/{idInstitution}")]
        public async Task<IActionResult> DeleteInstitution(long idInstitution)
        {
            var ret = await _app.Delete(idInstitution);
            return Ok(ret);
        }

    }
}
