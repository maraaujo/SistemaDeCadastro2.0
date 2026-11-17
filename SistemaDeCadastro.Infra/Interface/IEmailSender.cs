using System.Threading;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    /// <summary>
    /// Envio de e-mails transacionais (ex.: codigo de redefinicao de senha).
    /// A implementacao nunca lanca excecao para o chamador: falha de SMTP e tratada
    /// e registrada no log, para nao travar o fluxo nem revelar detalhes ao cliente.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Tenta enviar um e-mail. Retorna true se enviou, false se o SMTP nao esta
        /// configurado ou se o envio falhou (detalhe vai para o log).
        /// </summary>
        Task<bool> SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }
}
