using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPaymentApp _app;

        public PaymentController(IConfiguration configuration, IPaymentApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        [HttpGet("GetPaymentById")]
        public async Task<IActionResult> GetPaymentById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        [HttpGet("GetAllPayments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDTO entity)
        {
            // TODO: Ajustar IPaymentApp e PaymentApp para receber CreatePaymentDTO
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        [HttpPut("UpdatePayment")]
        public async Task<IActionResult> UpdatePayment(UpdatePaymentDTO entity)
        {
            // TODO: Ajustar IPaymentApp e PaymentApp para receber UpdatePaymentDTO
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        [HttpGet("DeletePayment/{idPayment}")]
        public async Task<IActionResult> DeletePayment(long idPayment)
        {
            var ret = await _app.Delete(idPayment);
            return Ok(ret);
        }
    }
}
