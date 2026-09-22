using System.Linq;

namespace SistemaDeCadastro.Domain.Validators
{
    /// <summary>
    /// Validação de CEP centralizada e reutilizável.
    /// Aceita o valor com ou sem máscara (ex.: "70000-000" ou "70000000"),
    /// remove os caracteres não numéricos e exige exatamente 8 dígitos.
    /// </summary>
    public static class CepValidator
    {
        public const string MensagemInvalido = "CEP inválido.";

        /// <summary>
        /// Remove tudo que não for dígito. Retorna string vazia se a entrada for nula.
        /// </summary>
        public static string Normalize(string? cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return string.Empty;

            return new string(cep.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Indica se o CEP é válido (8 dígitos após remover a máscara).
        /// Rejeita formatos claramente inválidos, como "00000000".
        /// </summary>
        public static bool IsValid(string? cep)
        {
            var digits = Normalize(cep);

            if (digits.Length != 8)
                return false;

            // Rejeita valores obviamente inválidos (todos os dígitos iguais a zero).
            if (digits.All(c => c == '0'))
                return false;

            return true;
        }
    }
}
