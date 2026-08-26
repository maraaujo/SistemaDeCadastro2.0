using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class LoginAccountApp : ILoginAccountApp
    {
        private readonly ILoginAccountRepository _repo;

        public LoginAccountApp(ILoginAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LoginAccountDTO>> GetAll() =>
            (await _repo.GetAll()).Select(ToDTO).ToList();

        public async Task<LoginAccountDTO> GetById(long id)
        {
            var account = (await _repo.FindBy(l => l.Id == id)).FirstOrDefault();

            return account == null ? null : ToDTO(account);
        }

        // Projeção segura: nunca expor PasswordHash em resposta de API.
        private static LoginAccountDTO ToDTO(LoginAccount account) => new()
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,
            Email = account.Email,
            UserType = account.UserType,
            InstitutionId = account.InstitutionId,
            LastLogin = account.LastLogin,
            Active = account.Active
        };

        public async Task<ApiResponse> Create(CreateLoginAccountDTO entity)
        {
            var ret = new ApiResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(entity.Email) || string.IsNullOrWhiteSpace(entity.Password))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Email e Senha devem ser preenchidos.";
                    return ret;
                }

                var existing = await _repo.FindBy(l => l.Email == entity.Email);

                if (existing.Any())
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Já existe uma conta com este email.";
                    return ret;
                }

                var hashed = BCrypt.Net.BCrypt.HashPassword(entity.Password);

                var loginAccount = new LoginAccount
                {
                    Email = entity.Email,
                    PasswordHash = hashed,
                    
                    UserType = entity.UserType,
                    InstitutionId = entity.InstitutionId,
                    LastLogin = null,
                    Active = true
                };
              
                await _repo.Create(loginAccount);
                loginAccount.UserId = loginAccount.Id;
                await _repo.Update(loginAccount);
                ret.Success = true;
                ret.Message = "Usuário criado com sucesso.";
                ret.Data = new
                {
                    Id = loginAccount.Id,
                    UserId = loginAccount.UserId,
                    Email = loginAccount.Email,
                    UserType = loginAccount.UserType,
                    InstitutionId = loginAccount.InstitutionId,
                    Active = loginAccount.Active
                };
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }

            return ret;
        }
        public async Task<ApiResponse> Update(UpdateLoginAccountDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                

                var existing = (await _repo.FindBy(l => l.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Conta não encontrada.";
                    return ret;
                }


                
                existing.Email = entity.Email;
                existing.Name = entity.Name;
                existing.UserType = entity.UserType;
                existing.LastLogin = entity.LastLogin;
                existing.Active = entity.Active ?? true;
               

                await _repo.Update(existing);

                ret.Success = true;
                ret.Message = "Usuário atualizado com sucesso.";
                ret.Data = ToDTO(existing);
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(l => l.Id == id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Conta não encontrada.";
                    return ret;
                }

                await _repo.Delete(existing);
                ret.Success = true;
                ret.Message = "Conta removida com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }
    }
}
