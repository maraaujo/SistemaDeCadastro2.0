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

        public async Task<List<LoginAccount>> GetAll() => await _repo.GetAll();

        public async Task<LoginAccount?> GetById(long id) => (await _repo.FindBy(l => l.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateLoginAccountDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(entity.Email) || string.IsNullOrWhiteSpace(entity.PasswordHash))
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

                var hashed = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);

                var loginAccount = new LoginAccount
                {
                    UserId = entity.UserId,
                    Email = entity.Email,
                    PasswordHash = hashed,
                    UserType = entity.UserType,
                    LastLogin = null,
                    Active = true
                };

                await _repo.Create(loginAccount);
                ret.Success = true;
                ret.Message = "Usuário criado com sucesso.";
                ret.Data = new
                {
                    Id = loginAccount.Id,
                    UserId = loginAccount.UserId,
                    Email = loginAccount.Email,
                    UserType = loginAccount.UserType,
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
                if (entity == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Entity is null.";
                    return ret;
                }

                var existing = (await _repo.FindBy(l => l.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Conta não encontrada.";
                    return ret;
                }


                existing.UserId = entity.UserId;
                existing.Email = entity.Email;
                existing.UserType = entity.UserType;
                existing.LastLogin = entity.LastLogin;
                existing.Active = entity.Active;
               

                await _repo.Update(existing);

                ret.Success = true;
                ret.Message = "Usuário atualizado com sucesso.";
                ret.Data = existing;
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
