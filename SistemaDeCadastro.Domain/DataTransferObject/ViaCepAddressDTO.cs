using System.Text.Json.Serialization;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    /// <summary>
    /// Representa o endereço retornado pela API pública ViaCEP
    /// (https://viacep.com.br/ws/{cep}/json/).
    /// </summary>
    public class ViaCepAddressDTO
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("ibge")]
        public string? Ibge { get; set; }

        [JsonPropertyName("ddd")]
        public string? Ddd { get; set; }

        // A ViaCEP responde { "erro": true } quando o CEP não existe.
        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
