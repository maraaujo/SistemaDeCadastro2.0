namespace SistemaDeCadastro.Domain.DataTransferObject
{
    // Inicio do fluxo "esqueci minha senha": o usuario informa o e-mail cadastrado
    // para receber o codigo de redefinicao.
    public class ForgotPasswordRequestDTO
    {
        public string Email { get; set; }
    }
}
