using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionApp _app;

        public SubscriptionController(ISubscriptionApp app)
        {
            _app = app;
        }

        [HttpGet("GetSubscriptionById")]
        public async Task<IActionResult> GetSubscriptionById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        [HttpGet("GetAllSubscriptions")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        [HttpPost("CreateSubscription")]
        public async Task<IActionResult> CreateSubscription(CreateSubscriptionDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdateSubscription")]
        public async Task<IActionResult> UpdateSubscription(UpdateSubscriptionDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeleteSubscription/{idSubscription}")]
        public async Task<IActionResult> DeleteSubscription(long idSubscription)
        {
            var ret = await _app.Delete(idSubscription);
            return Ok(ret);
        }

        [HttpGet("GetActiveSubscriptionByInstitution/{institutionId}")]
        public async Task<IActionResult> GetActiveByInstitutionId(long institutionId)
        {
            var ret = await _app.GetActiveByInstitutionId(institutionId);
            return Ok(ret);
        }
    }
}
