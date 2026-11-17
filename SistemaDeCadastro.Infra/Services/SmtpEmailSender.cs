using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Services
{
    /// <summary>
    /// Envio de e-mail por SMTP usando <see cref="SmtpClient"/> (System.Net.Mail).
    /// Em servidores como Gmail use a porta 587 com SSL (STARTTLS) e uma "senha de app".
    /// As configuracoes vem de <see cref="EmailSettings"/> (secao "Email" + SMTP_PASSWORD).
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public SmtpEmailSender(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task<bool> SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (!_settings.IsConfigured)
            {
                // Sem SMTP configurado nao ha como enviar; o chamador decide o que fazer.
                Console.Error.WriteLine("[EMAIL] SMTP nao configurado (secao 'Email'); e-mail nao enviado.");
                return false;
            }

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.FromAddress, _settings.FromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                };
                message.To.Add(new MailAddress(toEmail));

                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    EnableSsl = _settings.UseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                if (!string.IsNullOrWhiteSpace(_settings.Username))
                {
                    client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                }

                await client.SendMailAsync(message, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                // Falha de envio nao pode travar o fluxo nem vazar detalhes ao cliente.
                Console.Error.WriteLine($"[EMAIL] Falha ao enviar e-mail: {ex}");
                return false;
            }
        }
    }
}
