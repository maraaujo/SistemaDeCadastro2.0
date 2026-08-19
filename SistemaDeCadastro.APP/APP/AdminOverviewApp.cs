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

        public AdminOverviewApp(IAdminOverviewRepository repo)
        {
            _repo = repo;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

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
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
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
