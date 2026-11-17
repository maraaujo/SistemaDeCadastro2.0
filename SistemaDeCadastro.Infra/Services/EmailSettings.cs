namespace SistemaDeCadastro.Infra.Services
{
    // Configuracao do envio de e-mail (secao "Email" no appsettings). A senha pode vir
    // da variavel de ambiente SMTP_PASSWORD em vez de ficar no arquivo, seguindo o mesmo
    // padrao da senha do banco (DB_PASSWORD).
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Sistema de Gestão Assistencial";

        // So tentamos enviar se o minimo estiver configurado. Sem isso, o fluxo de
        // "esqueci minha senha" continua funcionando, mas sem disparar o e-mail.
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(SmtpHost) &&
            !string.IsNullOrWhiteSpace(FromAddress);
    }
}
