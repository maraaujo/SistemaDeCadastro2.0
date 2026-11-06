using Azure;
using Microsoft.Extensions.Configuration;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class AuthApp : IAuthApp
    {
        private readonly ILoginAccountRepository _loginAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly ILoginAttemptGuard _attemptGuard;

        private const string InvalidCredentials = "E-mail ou senha inválidos.";

        // Hash usado quando o e-mail nao existe, para a resposta levar o mesmo tempo
        // de uma senha errada e nao revelar quais e-mails tem conta.
        private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword("conta-inexistente-0");

        public AuthApp(ILoginAccountRepository loginAccountRepository, IConfiguration configuration, ILoginAttemptGuard attemptGuard)
        {
            _loginAccountRepository = loginAccountRepository;
            _configuration = configuration;
            _attemptGuard = attemptGuard;
        }

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
