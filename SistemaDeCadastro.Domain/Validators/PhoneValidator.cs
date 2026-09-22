using System.Collections.Generic;
using System.Linq;

namespace SistemaDeCadastro.Domain.Validators
{
    /// <summary>
    /// Validação de telefone brasileiro centralizada e reutilizável.
    /// Aceita entradas com ou sem máscara, por exemplo:
    ///   "(61) 99999-9999", "61999999999", "(61) 3333-3333", "6133333333".
    /// Remove a máscara e valida:
    ///   - Fixo: 10 dígitos (DDD + 8 dígitos, primeiro dígito do assinante entre 2 e 5).
    ///   - Celular: 11 dígitos (DDD + 9 dígitos, nono dígito iniciando em 9).
    ///   - DDD válido.
    /// Recomenda-se armazenar apenas os números (<see cref="Normalize"/>), deixando a
    /// máscara para apresentação no frontend.
    /// </summary>
    public static class PhoneValidator
    {
        public const string MensagemInvalido = "Telefone inválido.";

        // DDDs válidos no Brasil (Plano Nacional de Discagem da Anatel).
        private static readonly HashSet<int> ValidAreaCodes = new()
        {
            11, 12, 13, 14, 15, 16, 17, 18, 19,
            21, 22, 24, 27, 28,
            31, 32, 33, 34, 35, 37, 38,
            41, 42, 43, 44, 45, 46, 47, 48, 49,
            51, 53, 54, 55,
            61, 62, 63, 64, 65, 66, 67, 68, 69,
            71, 73, 74, 75, 77, 79,
            81, 82, 83, 84, 85, 86, 87, 88, 89,
            91, 92, 93, 94, 95, 96, 97, 98, 99
        };

        /// <summary>
        /// Remove tudo que não for dígito. Retorna string vazia se a entrada for nula.
        /// </summary>
        public static string Normalize(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            return new string(phone.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Indica se o telefone é válido (fixo com 10 dígitos ou celular com 11),
        /// aceitando entrada com ou sem máscara.
        /// </summary>
        public static bool IsValid(string? phone)
        {
            var digits = Normalize(phone);

            if (digits.Length != 10 && digits.Length != 11)
                return false;

            var areaCode = int.Parse(digits.Substring(0, 2));
            if (!ValidAreaCodes.Contains(areaCode))
                return false;

            var subscriberFirstDigit = digits[2];

            if (digits.Length == 11)
            {
                // Celular: nono dígito deve iniciar em 9.
                return subscriberFirstDigit == '9';
            }

            // Fixo: primeiro dígito do assinante entre 2 e 5.
            return subscriberFirstDigit >= '2' && subscriberFirstDigit <= '5';
        }
    }
}
