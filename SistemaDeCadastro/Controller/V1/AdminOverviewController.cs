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

        /// <summary>
        /// Recebe a camada de aplicacao do back-office por injecao.
        /// </summary>
        public AdminOverviewController(IAdminOverviewApp app)
        {
            _app = app;
        }

        /// <summary>
        /// GET api/AdminOverview/Kpis?period= - somente admin da plataforma (policy "SaasAdmin").
        /// Retorna os indicadores gerais da plataforma; period aceita 30d, 90d ou 12m (padrao 90d).
        /// Resposta: ApiResponse com os dados em Data.
        /// </summary>
        [HttpGet("Kpis")]
        public async Task<IActionResult> GetKpis([FromQuery] string? period)
        {
            var ret = await _app.GetKpis(period);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/AdminOverview/RevenueTrend?period= - somente admin da plataforma.
        /// Retorna a serie de evolucao da receita no periodo (30d, 90d ou 12m; padrao 90d).
        /// </summary>
        [HttpGet("RevenueTrend")]
        public async Task<IActionResult> GetRevenueTrend([FromQuery] string? period)
        {
            var ret = await _app.GetRevenueTrend(period);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/AdminOverview/SubscriptionMovement?period= - somente admin da plataforma.
        /// Retorna a movimentacao de assinaturas (entradas/saidas) no periodo (30d, 90d ou 12m).
        /// </summary>
        [HttpGet("SubscriptionMovement")]
        public async Task<IActionResult> GetSubscriptionMovement([FromQuery] string? period)
        {
            var ret = await _app.GetSubscriptionMovement(period);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/AdminOverview/RevenueByPlan?period= - somente admin da plataforma.
        /// Retorna a receita agrupada por plano no periodo (30d, 90d ou 12m).
        /// </summary>
        [HttpGet("RevenueByPlan")]
        public async Task<IActionResult> GetRevenueByPlan([FromQuery] string? period)
        {
            var ret = await _app.GetRevenueByPlan(period);
            return Ok(ret);
        }

        /// <summary>
        /// GET api/AdminOverview/SubscriptionStatus - somente admin da plataforma.
        /// Retorna a distribuicao atual das assinaturas por status (sem filtro de periodo).
        /// </summary>
        [HttpGet("SubscriptionStatus")]
        public async Task<IActionResult> GetSubscriptionStatus()
        {
            var ret = await _app.GetSubscriptionStatus();
            return Ok(ret);
        }

        /// <summary>
        /// GET api/AdminOverview/Institutions - somente admin da plataforma.
        /// Lista paginada de todas as instituicoes clientes, com busca por texto, filtro por status
        /// e plano e ordenacao (sort). Sem page/perPage usa pagina 1 com 10 itens.
        /// </summary>
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

        /// <summary>
        /// GET api/AdminOverview/Plans - somente admin da plataforma.
        /// Retorna a lista de planos usada pelo painel do back-office (ex.: nos filtros).
        /// </summary>
        [HttpGet("Plans")]
        public async Task<IActionResult> GetPlans()
        {
            var ret = await _app.GetPlans();
            return Ok(ret);
        }
    }
}
