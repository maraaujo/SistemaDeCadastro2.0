using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Controller.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlanController : ControllerBase
    {
        private readonly IPlanApp _app;

        /// <summary>
        /// Recebe a camada de aplicacao de planos por injecao.
        /// </summary>
        public PlanController(IPlanApp app)
        {
            _app = app;
        }

        /// <summary>
        /// GET api/Plan/GetPlanById?id= - qualquer usuario autenticado.
        /// Retorna ApiResponse com o plano de assinatura em Data.
        /// </summary>
        [HttpGet("GetPlanById")]
        public async Task<IActionResult> GetPlanById(long id)
        {
            var ret = await _app.GetById(id);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/Plan/GetAllPlans - qualquer usuario autenticado.
        /// Retorna ApiResponse com todos os planos de assinatura em Data.
        /// </summary>
        [HttpGet("GetAllPlans")]
        public async Task<IActionResult> GetAllPlans()
        {
            var ret = await _app.GetAll();
            return Ok(ret);
        }

        /// <summary>
        /// POST api/Plan/CreatePlan - somente admin da plataforma (policy "SaasAdmin").
        /// Cadastra um novo plano de assinatura; retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPost("CreatePlan")]
        public async Task<IActionResult> CreatePlan(CreatePlanDTO entity)
        {
            var ret = await _app.Create(entity);
            return Ok(ret);
        }

        /// <summary>
        /// PUT api/Plan/Update/{id} - somente admin da plataforma (policy "SaasAdmin").
        /// Atualiza o plano com os dados do corpo (o id usado e o do DTO); retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePlan(UpdatePlanDTO entity)
        {
            var ret = await _app.Update(entity);
            return Ok(ret);
        }

        /// <summary>
        /// DELETE api/Plan/DeletePlan/{idPlan} - somente admin da plataforma (policy "SaasAdmin").
        /// Exclui o plano; retorna ApiResponse.
        /// </summary>
        [Authorize(Policy = "SaasAdmin")]
        [HttpDelete("DeletePlan/{idPlan}")]
        public async Task<IActionResult> DeletePlan(long idPlan)
        {
            var ret = await _app.Delete(idPlan);
            return Ok(ret);
        }
    }
}
