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

        /// <summary>
        /// Recebe as dependencias por injecao (App de assinaturas e servico de autorizacao).
        /// </summary>
        public SubscriptionController(ISubscriptionApp app, IAuthorizationService authorizationService)
        {
            _app = app;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// GET api/Subscription/GetSubscriptionById?id= - somente admin da plataforma (policy "SaasAdmin").
        /// Retorna ApiResponse com a assinatura em Data.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("GetSubscriptionById")]
        public async Task<IActionResult> GetSubscriptionById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/Subscription/GetAllSubscriptions - somente admin da plataforma (policy "SaasAdmin").
        /// Retorna ApiResponse com todas as assinaturas (de todas as instituicoes) em Data.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpGet("GetAllSubscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        /// <summary>
        /// POST api/Subscription/CreateSubscription - somente admin da plataforma (policy "SaasAdmin").
        /// Cria a assinatura de uma instituicao em um plano; retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreateSubscription")]
        public async Task<IActionResult> CreateSubscription(CreateSubscriptionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Subscription/Update/{id} - somente admin da plataforma (policy "SaasAdmin").
        /// Atualiza a assinatura com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSubscription(UpdateSubscriptionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Subscription/DeleteSubscription/{idSubscription} - somente admin da plataforma.
        /// Exclui a assinatura; retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeleteSubscription/{idSubscription}")]
        public async Task<IActionResult> DeleteSubscription(long idSubscription)
        {
            var ret = await _app.Delete(idSubscription);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/Subscription/GetActiveSubscriptionByInstitution/{institutionId} - usuario autenticado.
        /// Retorna a assinatura ativa da instituicao. Usuario de instituicao so consulta a propria
        /// (senao 403/Forbid); o admin da plataforma consulta qualquer uma.
        /// </summary>
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
