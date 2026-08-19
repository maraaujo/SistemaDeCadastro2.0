using System.Text.Json.Serialization;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    /// <summary>
    /// Indicadores (KPIs) da Visão geral do back-office administrativo do SaaS.
    /// Os nomes JSON seguem o contrato consumido pelo dashboard (snake_case).
    /// </summary>
    public class AdminOverviewKpisDTO
    {
        [JsonPropertyName("period")]
        public string Period { get; set; } = string.Empty;

        [JsonPropertyName("revenue")]
        public AdminRevenueKpiDTO Revenue { get; set; } = new();

        [JsonPropertyName("active_subscriptions")]
        public AdminActiveSubscriptionsKpiDTO ActiveSubscriptions { get; set; } = new();

        [JsonPropertyName("new_subscriptions")]
        public AdminNewSubscriptionsKpiDTO NewSubscriptions { get; set; } = new();

        [JsonPropertyName("managed_residents")]
        public AdminManagedResidentsKpiDTO ManagedResidents { get; set; } = new();

        [JsonPropertyName("doses_this_month")]
        public AdminDosesKpiDTO DosesThisMonth { get; set; } = new();

        [JsonPropertyName("avg_adherence_pct")]
        public double AvgAdherencePct { get; set; }

        [JsonPropertyName("active_caregivers")]
        public int ActiveCaregivers { get; set; }
    }

    public class AdminRevenueKpiDTO
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("prev")]
        public decimal Prev { get; set; }

        // Nulo quando o período anterior é zero (evita divisão por zero -> exibe "—").
        [JsonPropertyName("change_pct")]
        public double? ChangePct { get; set; }
    }

    public class AdminActiveSubscriptionsKpiDTO
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("net_change")]
        public int NetChange { get; set; }

        [JsonPropertyName("trials")]
        public int Trials { get; set; }
    }

    public class AdminNewSubscriptionsKpiDTO
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("prev")]
        public int Prev { get; set; }

        [JsonPropertyName("change_pct")]
        public double? ChangePct { get; set; }
    }

    public class AdminManagedResidentsKpiDTO
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("avg_per_institution")]
        public int AvgPerInstitution { get; set; }
    }

    public class AdminDosesKpiDTO
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("on_time_pct")]
        public double OnTimePct { get; set; }
    }
}
