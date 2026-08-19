using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Infra.Interface
{
    /// <summary>
    /// Consultas agregadas (somente leitura) que alimentam a Visão geral do
    /// back-office administrativo do SaaS, todas sobre o banco real da aplicação.
    /// </summary>
    public interface IAdminOverviewRepository
    {
        Task<AdminOverviewKpisDTO> GetKpis(string period);
        Task<List<AdminRevenueTrendPointDTO>> GetRevenueTrend(string period);
        Task<List<AdminSubscriptionMovementPointDTO>> GetSubscriptionMovement(string period);
        Task<List<AdminRevenueByPlanDTO>> GetRevenueByPlan(string period);
        Task<List<AdminSubscriptionStatusDTO>> GetSubscriptionStatus();
        Task<AdminInstitutionsPageDTO> GetInstitutions(string? search, string? status, long? planId, string? sort, int page, int perPage);
        Task<List<AdminPlanOptionDTO>> GetPlans();
    }
}
