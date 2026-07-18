using Azure;
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

        public AuthApp(ILoginAccountRepository loginAccountRepository)
        {
            _loginAccountRepository = loginAccountRepository;
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
                response.Data = new LoginResponseDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    UserType = user.UserType,
                    Token = "token-temporario"
                };
                return response;
            }
            catch (Exception ex) {

                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
    }

}
