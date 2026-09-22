using System.Linq;

namespace SistemaDeCadastro.Domain.Validators
{
    /// <summary>
    /// Validação de CPF centralizada e reutilizável.
    /// Aceita o valor com ou sem máscara (ex.: "123.456.789-09" ou "12345678909"),
    /// remove os caracteres não numéricos e valida os dois dígitos verificadores.
    /// Não depende apenas de formato/Regex: recalcula os DVs.
    /// </summary>
    public static class CpfValidator
    {
        public const string MensagemInvalido = "CPF inválido.";

        /// <summary>
        /// Remove tudo que não for dígito. Retorna string vazia se a entrada for nula.
        /// </summary>
        public static string Normalize(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            return new string(cpf.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Indica se o CPF é válido, aceitando entrada com ou sem máscara.
        /// </summary>
        public static bool IsValid(string? cpf)
        {
            var digits = Normalize(cpf);

            // Precisa ter exatamente 11 dígitos.
            if (digits.Length != 11)
                return false;

            // Rejeita CPFs com todos os dígitos iguais (ex.: 11111111111).
            if (digits.Distinct().Count() == 1)
                return false;

            var numbers = digits.Select(c => c - '0').ToArray();

            // Primeiro dígito verificador.
            var firstCheck = CalculateCheckDigit(numbers, 9, 10);
            if (numbers[9] != firstCheck)
                return false;

            // Segundo dígito verificador.
            var secondCheck = CalculateCheckDigit(numbers, 10, 11);
            if (numbers[10] != secondCheck)
                return false;

            return true;
        }

        private static int CalculateCheckDigit(int[] numbers, int length, int startWeight)
        {
            var sum = 0;
            var weight = startWeight;

            for (var i = 0; i < length; i++)
            {
                sum += numbers[i] * weight;
                weight--;
            }

            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }
    }
}
