using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IPaymentApp _app;

        /// <summary>
        /// Recebe as dependencias por injecao (configuracao e camada de aplicacao de pagamentos).
        /// </summary>
        public PaymentController(IConfiguration configuration, IPaymentApp app)
        {
            this._configuration = configuration;
            this._app = app;
        }

        /// <summary>
        /// GET api/Payment/GetPaymentById?id= - qualquer usuario autenticado.
        /// Retorna o pagamento (de um agendamento) pelo id, ou null.
        /// </summary>
        [HttpGet("GetPaymentById")]
        public async Task<IActionResult> GetPaymentById(long id)
        {
            var item = await _app.GetById(id);
            return Ok(item);
        }

        /// <summary>
        /// GET api/Payment/GetAllPayments - qualquer usuario autenticado.
        /// Retorna todos os pagamentos visiveis para a instituicao do usuario.
        /// </summary>
        [HttpGet("GetAllPayments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var items = await _app.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// POST api/Payment/CreatePayment - qualquer usuario autenticado.
        /// Registra um pagamento; retorna ApiResponse com Success/ErrorMessage.
        /// </summary>
        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Payment/Update/{id} - qualquer usuario autenticado.
        /// Atualiza o pagamento com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePayment(UpdatePaymentDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Payment/DeletePayment/{idPayment} - qualquer usuario autenticado.
        /// Exclui o pagamento; retorna ApiResponse.
        /// </summary>
        [HttpDelete("DeletePayment/{idPayment}")]
        public async Task<IActionResult> DeletePayment(long idPayment)
        {
            var ret = await _app.Delete(idPayment);
            return Ok(ret);
        }
    }
}
