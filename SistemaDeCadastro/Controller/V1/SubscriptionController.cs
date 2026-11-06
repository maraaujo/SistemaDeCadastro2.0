using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionApp _app;
        private readonly IAuthorizationService _authorizationService;

        public SubscriptionController(ISubscriptionApp app, IAuthorizationService authorizationService)
        {
            _app = app;
            _authorizationService = authorizationService;
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("GetSubscriptionById")]
        public async Task<IActionResult> GetSubscriptionById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("GetAllSubscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateSubscription")]
        public async Task<IActionResult> CreateSubscription(CreateSubscriptionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSubscription(UpdateSubscriptionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteSubscription/{idSubscription}")]
        public async Task<IActionResult> DeleteSubscription(long idSubscription)
        {
            var ret = await _app.Delete(idSubscription);
            return Ok(ret);
        }

        [HttpGet("GetActiveSubscriptionByInstitution/{institutionId}")]
        public async Task<IActionResult> GetActiveByInstitutionId(long institutionId)
        {
            // Instituicao consulta apenas a propria assinatura.
            if (!await InstitutionController.CanAccessInstitution(User, institutionId, _authorizationService))
                return Forbid();

            var ret = await _app.GetActiveByInstitutionId(institutionId);
            return Ok(ret);
        }
    }
}
