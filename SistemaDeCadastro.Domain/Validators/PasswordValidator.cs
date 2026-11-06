using System.Linq;

namespace SistemaDeCadastro.Domain.Validators
{
    /// <summary>
    /// Regra minima de senha das contas de acesso: pelo menos 8 caracteres,
    /// com ao menos uma letra e um numero.
    /// </summary>
    public static class PasswordValidator
    {
        public const int MinLength = 8;

        public const string MensagemInvalido =
            "A senha deve ter pelo menos 8 caracteres, com letras e números.";

        public static bool IsValid(string? password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinLength)
                return false;

            return password.Any(char.IsLetter) && password.Any(char.IsDigit);
        }
    }
}
