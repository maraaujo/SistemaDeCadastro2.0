using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    // Gestao da plataforma: dados de todas as instituicoes, so o admin da plataforma.
    [Authorize(Policy = "SaasAdmin")]
    public class SubscriptionPaymentController : ControllerBase
    {
        private readonly ISubscriptionPaymentApp _app;

        /// <summary>
        /// Recebe a camada de aplicacao de pagamentos de assinatura por injecao.
        /// </summary>
        public SubscriptionPaymentController(ISubscriptionPaymentApp app)
        {
            _app = app;
        }

        /// <summary>
        /// GET api/SubscriptionPayment/GetSubscriptionPaymentById?id= - somente admin da plataforma (policy da classe).
        /// Retorna ApiResponse com o pagamento de assinatura em Data.
        /// </summary>
        [HttpGet("GetSubscriptionPaymentById")]
        public async Task<IActionResult> GetSubscriptionPaymentById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/SubscriptionPayment/GetAllSubscriptionsPayment - somente admin da plataforma.
        /// Retorna ApiResponse com todos os pagamentos de assinatura em Data.
        /// </summary>
        [HttpGet("GetAllSubscriptionsPayment")]
        public async Task<IActionResult> GetAllSubscriptionsPayment()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        /// <summary>
        /// POST api/SubscriptionPayment/CreateSubscriptionPayment - somente admin da plataforma.
        /// Registra um pagamento de assinatura; retorna ApiResponse.
        /// </summary>
        [HttpPost("CreateSubscriptionPayment")]
        public async Task<IActionResult> CreateSubscriptionPayment(CreateSubscriptionPaymentDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/SubscriptionPayment/Update/{id} - somente admin da plataforma (o id usado e o do DTO).
        /// Atualiza o pagamento de assinatura; retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSubscriptionPayment(UpdateSubscriptionPaymentDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/SubscriptionPayment/DeleteSubscriptionPayment/{id} - somente admin da plataforma.
        /// Exclui o pagamento de assinatura; retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeleteSubscriptionPayment/{idSubscriptionPayment}")]
        public async Task<IActionResult> DeleteSubscriptionPayment(long idSubscriptionPayment)
        {
            var ret = await _app.Delete(idSubscriptionPayment);
            return Ok(ret);
        }

    }
}
