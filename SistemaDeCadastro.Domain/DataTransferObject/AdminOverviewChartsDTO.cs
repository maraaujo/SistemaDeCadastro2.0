using System.Text.Json.Serialization;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    /// <summary>Ponto mensal da série de evolução do faturamento.</summary>
    public class AdminRevenueTrendPointDTO
    {
        [JsonPropertyName("month")]
        public string Month { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }

    /// <summary>Ponto mensal do movimento de assinaturas (novas x canceladas).</summary>
    public class AdminSubscriptionMovementPointDTO
    {
        [JsonPropertyName("month")]
        public string Month { get; set; } = string.Empty;

        [JsonPropertyName("new")]
        public int New { get; set; }

        [JsonPropertyName("canceled")]
        public int Canceled { get; set; }
    }

    /// <summary>Faturamento agregado por plano no período.</summary>
    public class AdminRevenueByPlanDTO
    {
        [JsonPropertyName("plan")]
        public string Plan { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("accounts")]
        public int Accounts { get; set; }

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
    }

    /// <summary>Contagem de assinaturas por situação (para o donut).</summary>
    public class AdminSubscriptionStatusDTO
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
