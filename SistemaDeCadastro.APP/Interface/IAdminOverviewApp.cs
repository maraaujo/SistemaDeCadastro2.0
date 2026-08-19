using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAdminOverviewApp
    {
        Task<ApiResponse> GetKpis(string? period);
        Task<ApiResponse> GetRevenueTrend(string? period);
        Task<ApiResponse> GetSubscriptionMovement(string? period);
        Task<ApiResponse> GetRevenueByPlan(string? period);
        Task<ApiResponse> GetSubscriptionStatus();
        Task<ApiResponse> GetInstitutions(string? search, string? status, long? planId, string? sort, int? page, int? perPage);
        Task<ApiResponse> GetPlans();
    }
}
