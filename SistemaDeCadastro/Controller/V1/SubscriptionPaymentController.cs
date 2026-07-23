using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionPaymentController : ControllerBase
    {
        private readonly ISubscriptionPaymentApp _app;

        public SubscriptionPaymentController(ISubscriptionPaymentApp app)
        {
            _app = app;
        }

        [HttpGet("GetSubscriptionPaymentById")]
        public async Task<IActionResult> GetSubscriptionPaymentById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        [HttpGet("GetAllSubscriptionsPayment")]
        public async Task<IActionResult> GetAllSubscriptionsPayment()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        [HttpPost("CreateSubscriptionPayment")]
        public async Task<IActionResult> CreateSubscriptionPayment(CreateSubscriptionPaymentDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateSubscriptionPayment")]
        public async Task<IActionResult> UpdateSubscriptionPayment(UpdateSubscriptionPaymentDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteSubscriptionPayment/{idSubscriptionPayment}")]
        public async Task<IActionResult> DeleteSubscriptionPayment(long idSubscriptionPayment)
        {
            var ret = await _app.Delete(idSubscriptionPayment);
            return Ok(ret);
        }

    }
}
