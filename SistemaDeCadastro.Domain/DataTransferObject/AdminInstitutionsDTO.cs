using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    /// <summary>Opção de plano (usada nos filtros do dashboard).</summary>
    public class AdminPlanOptionDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("monthlyPrice")]
        public decimal MonthlyPrice { get; set; }
    }

    /// <summary>Linha da tabela de instituições do back-office.</summary>
    public class AdminInstitutionRowDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("planId")]
        public long? PlanId { get; set; }

        [JsonPropertyName("plan")]
        public AdminPlanOptionDTO? Plan { get; set; }

        // Código de situação já normalizado (active | trial | past_due | canceling | ...).
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("monthlyRevenue")]
        public decimal MonthlyRevenue { get; set; }

        [JsonPropertyName("residents")]
        public int Residents { get; set; }

        [JsonPropertyName("adherencePct")]
        public double AdherencePct { get; set; }

        // Data da próxima renovação / fim de vigência (ISO) ou nulo quando não houver.
        [JsonPropertyName("renewsAt")]
        public string? RenewsAt { get; set; }
    }

    /// <summary>Página de instituições (paginação server-side).</summary>
    public class AdminInstitutionsPageDTO
    {
        [JsonPropertyName("items")]
        public List<AdminInstitutionRowDTO> Items { get; set; } = new();

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("perPage")]
        public int PerPage { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
