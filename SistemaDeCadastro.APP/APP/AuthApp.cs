using Azure;
using Microsoft.Extensions.Configuration;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class AuthApp : IAuthApp
    {
        private readonly ILoginAccountRepository _loginAccountRepository;
        private readonly IConfiguration _configuration;

        public AuthApp(ILoginAccountRepository loginAccountRepository, IConfiguration configuration)
        {
            _loginAccountRepository = loginAccountRepository;
            _configuration = configuration;
        }



        public async Task<ApiResponse> Login(LoginRequestDTO login)
        {
            ApiResponse response = new ApiResponse();
            // usuário no banco de dados
            LoginAccount user = await _loginAccountRepository.GetByEmail(login.Email);
            try
            {
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "E-mail ou senha inválidos.";
                    return response;
                }
                if (user.Active == false)
                {
                    response.Success = false;
                    response.Message = "Usuário inativo.";
                    return response;
                }
                if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "E-mail ou senha inválidos.";
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
                    UserType = user.UserType,
                    Token = token
                };
                return response;
            }
            catch (Exception ex) {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
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
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.UserType ?? string.Empty)
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
