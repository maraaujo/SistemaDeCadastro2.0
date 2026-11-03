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

        public InstitutionController(IInstitutionApp app, IAuthorizationService authorizationService)
        {
            _app = app;
            _authorizationService = authorizationService;
        }

        // Usuario de instituicao le apenas a propria instituicao (ex.: tela de perfil);
        // o admin da plataforma (policy SaasAdmin) le qualquer uma.
        [HttpGet("GetInstitutionById/{id}")]
        public async Task<IActionResult> GetInstitutionById(long id)
        {
            if (!await CanAccessInstitution(User, id, _authorizationService))
                return Forbid();

            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        internal static async Task<bool> CanAccessInstitution(
            System.Security.Claims.ClaimsPrincipal user, long institutionId, IAuthorizationService authorizationService)
        {
            var claim = user.FindFirst("institutionId")?.Value ?? user.FindFirst("InstitutionId")?.Value;
            if (long.TryParse(claim, out var ownInstitutionId))
                return ownInstitutionId == institutionId;

            return (await authorizationService.AuthorizeAsync(user, "SaasAdmin")).Succeeded;
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("GetAllInstitutions")]
        public async Task<IActionResult> GetAllInstitutions()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateInstitution")]
        public async Task<IActionResult> CreateInstitution(CreateInstitutionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateInstitution(UpdateInstitutionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("DeleteInstitution/{idInstitution}")]
        public async Task<IActionResult> DeleteInstitution(long idInstitution)
        {
            var ret = await _app.Delete(idInstitution);
            return Ok(ret);
        }

    }
}
