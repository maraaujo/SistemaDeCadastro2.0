using System;
using System.Threading.Tasks;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    // Servico de aplicacao do back-office (somente leitura). Valida o parametro de
    // periodo e delega as agregacoes ao repositorio, sempre sobre o banco real.
    public class AdminOverviewApp : IAdminOverviewApp
    {
        private readonly IAdminOverviewRepository _repo;

        private static readonly string[] AllowedPeriods = { "30d", "90d", "12m" };
        private const string DefaultPeriod = "90d";

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de agregacoes do back-office.
        /// </summary>
        public AdminOverviewApp(IAdminOverviewRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Devolve em Data os indicadores (KPIs) gerais da plataforma no periodo informado
        /// (30d, 90d ou 12m; valor invalido ou vazio vira 90d).
        /// </summary>
        public async Task<ApiResponse> GetKpis(string? period)
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetKpis(NormalizePeriod(period));
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a serie de evolucao da receita no periodo informado (normalizado para 30d, 90d ou 12m).
        /// </summary>
        public async Task<ApiResponse> GetRevenueTrend(string? period)
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetRevenueTrend(NormalizePeriod(period));
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a movimentacao de assinaturas (entradas/saidas) no periodo informado (normalizado para 30d, 90d ou 12m).
        /// </summary>
        public async Task<ApiResponse> GetSubscriptionMovement(string? period)
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetSubscriptionMovement(NormalizePeriod(period));
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a receita agrupada por plano no periodo informado (normalizado para 30d, 90d ou 12m).
        /// </summary>
        public async Task<ApiResponse> GetRevenueByPlan(string? period)
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetRevenueByPlan(NormalizePeriod(period));
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a distribuicao atual das assinaturas por status (sem filtro de periodo).
        /// </summary>
        public async Task<ApiResponse> GetSubscriptionStatus()
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetSubscriptionStatus();
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a lista paginada de instituicoes, com busca por texto, filtro por status e plano e ordenacao.
        /// Se page/perPage nao forem informados, usa pagina 1 com 10 itens.
        /// </summary>
        public async Task<ApiResponse> GetInstitutions(string? search, string? status, long? planId, string? sort, int? page, int? perPage)
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetInstitutions(
                    search,
                    status,
                    planId,
                    sort,
                    page ?? 1,
                    perPage ?? 10);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a lista de planos usada pelo painel do back-office (ex.: para filtros).
        /// </summary>
        public async Task<ApiResponse> GetPlans()
        {
            var ret = new ApiResponse();
            try
            {
                ret.Data = await _repo.GetPlans();
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        // Periodo aceito: 30d | 90d | 12m. Qualquer outro valor cai no padrao (90d).
        private static string NormalizePeriod(string? period)
        {
            if (string.IsNullOrWhiteSpace(period))
                return DefaultPeriod;

            var normalized = period.Trim().ToLowerInvariant();
            return Array.IndexOf(AllowedPeriods, normalized) >= 0 ? normalized : DefaultPeriod;
        }
    }
}
