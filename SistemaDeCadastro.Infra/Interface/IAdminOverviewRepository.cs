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
        /// <summary>Calcula os indicadores (KPIs) do painel: faturamento, assinaturas, idosos, doses, adesao e cuidadores no periodo ("30d", "90d" ou "12m").</summary>
        Task<AdminOverviewKpisDTO> GetKpis(string period);
        /// <summary>Faturamento pago mes a mes nos ultimos 12 meses (o parametro period nao e usado).</summary>
        Task<List<AdminRevenueTrendPointDTO>> GetRevenueTrend(string period);
        /// <summary>Quantidade de assinaturas novas e canceladas por mes dentro do periodo informado.</summary>
        Task<List<AdminSubscriptionMovementPointDTO>> GetSubscriptionMovement(string period);
        /// <summary>Faturamento pago no periodo e numero de instituicoes ativas agrupados por plano, com o percentual de cada plano.</summary>
        Task<List<AdminRevenueByPlanDTO>> GetRevenueByPlan(string period);
        /// <summary>Contagem de assinaturas por situacao normalizada (active, trial, past_due, canceling...).</summary>
        Task<List<AdminSubscriptionStatusDTO>> GetSubscriptionStatus();
        /// <summary>Lista paginada das instituicoes (tenants) com plano atual, situacao, receita, idosos e adesao, com busca, filtros e ordenacao.</summary>
        Task<AdminInstitutionsPageDTO> GetInstitutions(string? search, string? status, long? planId, string? sort, int page, int perPage);
        /// <summary>Lista os planos ativos ordenados pelo preco mensal (opcoes do filtro de plano).</summary>
        Task<List<AdminPlanOptionDTO>> GetPlans();
    }
}
