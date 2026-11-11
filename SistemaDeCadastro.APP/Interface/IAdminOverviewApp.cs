using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAdminOverviewApp
    {
        /// <summary>Devolve os indicadores gerais do back-office no periodo (30d, 90d ou 12m; padrao 90d).</summary>
        Task<ApiResponse> GetKpis(string? period);
        /// <summary>Devolve a evolucao da receita da plataforma no periodo.</summary>
        Task<ApiResponse> GetRevenueTrend(string? period);
        /// <summary>Devolve a movimentacao de assinaturas no periodo.</summary>
        Task<ApiResponse> GetSubscriptionMovement(string? period);
        /// <summary>Devolve a receita agrupada por plano no periodo.</summary>
        Task<ApiResponse> GetRevenueByPlan(string? period);
        /// <summary>Devolve a distribuicao atual das assinaturas por status.</summary>
        Task<ApiResponse> GetSubscriptionStatus();
        /// <summary>Devolve a lista paginada de instituicoes com busca, filtros por status/plano e ordenacao.</summary>
        Task<ApiResponse> GetInstitutions(string? search, string? status, long? planId, string? sort, int? page, int? perPage);
        /// <summary>Devolve a lista de planos para o painel do back-office.</summary>
        Task<ApiResponse> GetPlans();
    }
}
