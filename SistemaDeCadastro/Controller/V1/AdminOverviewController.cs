using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaDeCadastro.APP.Interface;

namespace SistemaDeCadastro.Controller.V1
{
    // Back-office administrativo do SaaS (Visao geral). Somente leitura.
    // Acesso restrito ao administrador da plataforma pela policy "SaasAdmin"
    // (usuario autenticado, com papel administrativo e sem vinculo a uma instituicao).
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SaasAdmin")]
    public class AdminOverviewController : ControllerBase
    {
        private readonly IAdminOverviewApp _app;

        public AdminOverviewController(IAdminOverviewApp app)
        {
            _app = app;
        }

        [HttpGet("Kpis")]
        public async Task<IActionResult> GetKpis([FromQuery] string? period)
        {
            var ret = await _app.GetKpis(period);
            return Ok(ret);
        }

        [HttpGet("RevenueTrend")]
        public async Task<IActionResult> GetRevenueTrend([FromQuery] string? period)
        {
            var ret = await _app.GetRevenueTrend(period);
            return Ok(ret);
        }

        [HttpGet("SubscriptionMovement")]
        public async Task<IActionResult> GetSubscriptionMovement([FromQuery] string? period)
        {
            var ret = await _app.GetSubscriptionMovement(period);
            return Ok(ret);
        }

        [HttpGet("RevenueByPlan")]
        public async Task<IActionResult> GetRevenueByPlan([FromQuery] string? period)
        {
            var ret = await _app.GetRevenueByPlan(period);
            return Ok(ret);
        }

        [HttpGet("SubscriptionStatus")]
        public async Task<IActionResult> GetSubscriptionStatus()
        {
            var ret = await _app.GetSubscriptionStatus();
            return Ok(ret);
        }

        [HttpGet("Institutions")]
        public async Task<IActionResult> GetInstitutions(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] long? plan,
            [FromQuery] string? sort,
            [FromQuery] int? page,
            [FromQuery] int? perPage)
        {
            var ret = await _app.GetInstitutions(search, status, plan, sort, page, perPage);
            return Ok(ret);
        }

        [HttpGet("Plans")]
        public async Task<IActionResult> GetPlans()
        {
            var ret = await _app.GetPlans();
            return Ok(ret);
        }
    }
}
