using Azure;
using Microsoft.Extensions.Configuration;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Validators;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class AuthApp : IAuthApp
    {
        private readonly ILoginAccountRepository _loginAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly ILoginAttemptGuard _attemptGuard;
        private readonly IEmailSender _emailSender;

        private const string InvalidCredentials = "E-mail ou senha inválidos.";

        // Resposta neutra do "esqueci minha senha": e sempre a mesma, exista a conta ou
        // nao, para nao revelar quais e-mails estao cadastrados.
        private const string ForgotPasswordNeutralMessage =
            "Se o e-mail estiver cadastrado, você receberá um código para redefinir a senha.";

        // Tempo de validade do codigo enviado por e-mail.
        private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromMinutes(15);

        // Hash usado quando o e-mail nao existe, para a resposta levar o mesmo tempo
        // de uma senha errada e nao revelar quais e-mails tem conta.
        private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword("conta-inexistente-0");

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de contas, a configuracao (secao Jwt) e o controle de tentativas de login.
        /// </summary>
        public AuthApp(ILoginAccountRepository loginAccountRepository, IConfiguration configuration, ILoginAttemptGuard attemptGuard, IEmailSender emailSender)
        {
            _loginAccountRepository = loginAccountRepository;
            _configuration = configuration;
            _attemptGuard = attemptGuard;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Valida a sessao a cada requisicao: le o UserId do token, confere se a conta existe e esta ativa
        /// e se o papel (Role) e a instituicao do token ainda sao os mesmos da conta no banco.
        /// Retorna false se qualquer um deles mudou (ex.: conta desativada ou papel trocado depois do login).
        /// </summary>
        public async Task<bool> IsSessionValid(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (!long.TryParse(principal.FindFirst("UserId")?.Value, out var userId))
                return false;

            var account = await _loginAccountRepository.GetByUserId(userId);
            if (account == null || !account.Active)
                return false;

            var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;
            if (!string.Equals(account.UserType ?? string.Empty, role, StringComparison.Ordinal))
                return false;

            var institutionClaim = principal.FindFirst("institutionId")?.Value;
            long? tokenInstitutionId = long.TryParse(institutionClaim, out var parsed) ? parsed : null;
            return account.InstitutionId == tokenInstitutionId;
        }



        /// <summary>
        /// Faz o login por e-mail e senha. Recusa se o e-mail estiver bloqueado por excesso de tentativas,
        /// confere a senha com BCrypt (usando um hash falso quando o e-mail nao existe, para nao revelar contas)
        /// e registra a falha no controle de tentativas. Com senha correta: zera as tentativas, recusa conta inativa,
        /// grava o ultimo login e devolve em Data os dados do usuario e o token JWT. Mensagens vao em Message.
        /// </summary>
        public async Task<ApiResponse> Login(LoginRequestDTO login)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(login?.Email) || string.IsNullOrEmpty(login.Password))
                {
                    response.Success = false;
                    response.Message = InvalidCredentials;
                    return response;
                }

                // Protecao contra forca bruta: apos varias senhas erradas o e-mail fica
                // bloqueado por um tempo, exista a conta ou nao.
                var remaining = _attemptGuard.GetRemainingLockout(login.Email);
                if (remaining != null)
                {
                    response.Success = false;
                    response.Message =
                        $"Muitas tentativas de login. Tente novamente em {Math.Ceiling(remaining.Value.TotalMinutes)} minuto(s).";
                    return response;
                }

                // usuário no banco de dados
                LoginAccount? user = await _loginAccountRepository.GetByEmail(login.Email);

                var passwordOk = BCrypt.Net.BCrypt.Verify(login.Password, user?.PasswordHash ?? DummyHash);
                if (user == null || !passwordOk)
                {
                    _attemptGuard.RegisterFailure(login.Email);
                    response.Success = false;
                    response.Message = InvalidCredentials;
                    return response;
                }

                _attemptGuard.Reset(login.Email);

                // So informa que a conta esta inativa para quem acertou a senha.
                if (user.Active == false)
                {
                    response.Success = false;
                    response.Message = "Usuário inativo.";
                    return response;
                }
                
                    //atualiza o último login do usuário
                    user.LastLogin = DateTime.Now;
                // atualiza último login no banco de dados
                await _loginAccountRepository.Update(user);

                //se não houver erros, retorna sucesso e dados do usuário
                response.Success = true;
                response.Message = "Login realizado com sucesso.";
                var token = GenerateJwtToken(user);
                response.Data = new LoginResponseDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Name = user.Name,
                    UserType = user.UserType,
                    InstitutionId = user.InstitutionId,
                    Token = token
                };
                return response;
            }
            catch (Exception ex) {
                response.Success = false;
                response.Message = ApiErrors.From(ex);
                return response;
            }
        }

        // Passo 1 do "esqueci minha senha": gera um codigo, guarda so o hash + expiracao
        // na conta e envia o codigo por e-mail. A resposta e sempre neutra (nao revela se
        // o e-mail existe) e nunca devolve o codigo ao cliente.
        public async Task<ApiResponse> RequestPasswordReset(ForgotPasswordRequestDTO request)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Email))
                {
                    response.Success = true;
                    response.Message = ForgotPasswordNeutralMessage;
                    return response;
                }

                var user = await _loginAccountRepository.GetByEmail(request.Email.Trim());

                // So gera/envia codigo para conta existente e ativa. Em qualquer outro
                // caso a resposta e a mesma, para nao revelar a existencia da conta.
                if (user != null && user.Active)
                {
                    var code = GenerateResetCode();
                    user.PasswordResetTokenHash = HashCode(code);
                    user.PasswordResetTokenExpiresAt = DateTime.UtcNow.Add(ResetTokenLifetime);
                    await _loginAccountRepository.Update(user);

                    await _emailSender.SendAsync(
                        user.Email,
                        "Redefinição de senha",
                        BuildResetEmailBody(user.Name, code));
                }

                response.Success = true;
                response.Message = ForgotPasswordNeutralMessage;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ApiErrors.From(ex);
                return response;
            }
        }

        // Passo 2 do "esqueci minha senha": confere o codigo (hash + validade), aplica a
        // nova senha e invalida o codigo (uso unico).
        public async Task<ApiResponse> ResetPassword(ResetPasswordRequestDTO request)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (request == null ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Code) ||
                    string.IsNullOrEmpty(request.NewPassword))
                {
                    response.Success = false;
                    response.Message = "Código ou nova senha inválidos.";
                    return response;
                }

                if (!PasswordValidator.IsValid(request.NewPassword))
                {
                    response.Success = false;
                    response.Message = PasswordValidator.MensagemInvalido;
                    return response;
                }

                var user = await _loginAccountRepository.GetByEmail(request.Email.Trim());

                var providedHash = HashCode(request.Code.Trim());
                var tokenMatches =
                    user?.PasswordResetTokenHash != null &&
                    CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(user.PasswordResetTokenHash),
                        Encoding.UTF8.GetBytes(providedHash));

                var tokenValid =
                    tokenMatches &&
                    user!.PasswordResetTokenExpiresAt != null &&
                    user.PasswordResetTokenExpiresAt.Value > DateTime.UtcNow;

                if (user == null || !user.Active || !tokenValid)
                {
                    response.Success = false;
                    response.Message = "Código inválido ou expirado. Solicite um novo.";
                    return response;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                // Codigo e de uso unico: invalida apos redefinir.
                user.PasswordResetTokenHash = null;
                user.PasswordResetTokenExpiresAt = null;
                await _loginAccountRepository.Update(user);

                // Libera eventual bloqueio por tentativas de login desta conta.
                _attemptGuard.Reset(user.Email);

                response.Success = true;
                response.Message = "Senha redefinida com sucesso. Faça login com a nova senha.";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ApiErrors.From(ex);
                return response;
            }
        }

        // Codigo numerico de 6 digitos, gerado com fonte criptografica.
        private static string GenerateResetCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        // Guardamos apenas o hash do codigo (nunca o codigo em texto).
        private static string HashCode(string code)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(bytes);
        }

        private static string BuildResetEmailBody(string? name, string code)
        {
            var saudacao = string.IsNullOrWhiteSpace(name) ? "Olá" : $"Olá, {name}";
            return $@"
<div style=""font-family:Arial,Helvetica,sans-serif;color:#1f2933;line-height:1.5"">
  <p>{saudacao},</p>
  <p>Recebemos um pedido para redefinir a senha da sua conta no Sistema de Gestão Assistencial.</p>
  <p>Use o código abaixo para criar uma nova senha:</p>
  <p style=""font-size:28px;font-weight:bold;letter-spacing:4px;margin:16px 0"">{code}</p>
  <p>O código é válido por {ResetTokenLifetime.TotalMinutes:0} minutos. Se você não solicitou a redefinição, ignore este e-mail.</p>
</div>";
        }

        /// <summary>
        /// Gera o token JWT assinado (HmacSha256) com as claims UserId, e-mail, papel (Role) e institutionId,
        /// usando chave, emissor, audiencia e validade (padrao 60 minutos) da secao "Jwt" da configuracao.
        /// </summary>
        private string GenerateJwtToken(LoginAccount user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresIn = int.Parse(jwtSection["ExpiresInMinutes"] ?? "60");

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("UserId", user.UserId.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? string.Empty),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.UserType ?? string.Empty),
                new System.Security.Claims.Claim("institutionId", user.InstitutionId.ToString()),
            };

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(token);
        }
    }

}
