namespace SistemaDeCadastro.Domain.DataTransferObject
{
    // Conclusao do fluxo "esqueci minha senha": o usuario envia o codigo recebido
    // por e-mail junto da nova senha.
    public class ResetPasswordRequestDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
