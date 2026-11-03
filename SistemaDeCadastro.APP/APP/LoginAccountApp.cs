using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    // Contas de acesso nao usam HasQueryFilter: Appointment, Payment, UserPermission e
    // AccessLog tem navegacao obrigatoria para LoginAccount e um filtro global faria
    // esses registros sumirem dos Includes. O isolamento entre instituicoes e feito aqui:
    // - admin da plataforma (papel Administrador sem instituicao): ve e gerencia todas;
    // - administrador de instituicao: ve e gerencia so as contas da propria instituicao;
    // - demais usuarios: so leem/editam a propria conta, sem alterar papel ou status.
    public class LoginAccountApp : ILoginAccountApp
    {
        public const string AdminRole = "Administrador";

        private readonly ILoginAccountRepository _repo;
        private readonly ICurrentUserServiceContext _currentUser;

        public LoginAccountApp(ILoginAccountRepository repo, ICurrentUserServiceContext currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        private bool IsAdmin =>
            string.Equals(_currentUser.Role, AdminRole, StringComparison.OrdinalIgnoreCase);

        private bool IsPlatformAdmin => _currentUser.IsPlatformAdmin;

        private bool IsInstitutionAdmin => IsAdmin && _currentUser.InstitutionId.HasValue;

        private bool IsOwnAccount(LoginAccount account) =>
            _currentUser.UserId.HasValue && account.UserId == _currentUser.UserId.Value;

        // Pode administrar a conta (alterar papel/status, apagar).
        private bool CanManage(LoginAccount account) =>
            IsPlatformAdmin ||
            (IsInstitutionAdmin && account.InstitutionId == _currentUser.InstitutionId);

        private bool CanView(LoginAccount account) => CanManage(account) || IsOwnAccount(account);

        public async Task<List<LoginAccountDTO>> GetAll()
        {
            List<LoginAccount> accounts;

            if (IsPlatformAdmin)
            {
                accounts = await _repo.GetAll();
            }
            else if (IsInstitutionAdmin)
            {
                var institutionId = _currentUser.InstitutionId;
                accounts = await _repo.FindBy(l => l.InstitutionId == institutionId);
            }
            else if (_currentUser.UserId.HasValue)
            {
                var userId = _currentUser.UserId.Value;
                accounts = await _repo.FindBy(l => l.UserId == userId);
            }
            else
            {
                accounts = new List<LoginAccount>();
            }

            return accounts.Select(ToDTO).ToList();
        }

        public async Task<LoginAccountDTO> GetById(long id)
        {
            var account = (await _repo.FindBy(l => l.Id == id)).FirstOrDefault();

            return account == null || !CanView(account) ? null : ToDTO(account);
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
                if (!IsAdmin)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Somente administradores podem criar contas de acesso.";
                    return ret;
                }

                if (string.IsNullOrWhiteSpace(entity.Email) || string.IsNullOrWhiteSpace(entity.Password))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Email e Senha devem ser preenchidos.";
                    return ret;
                }

                // A coluna contas_login.nome e obrigatoria: sem esta checagem o banco
                // devolvia um erro tecnico de NOT NULL em vez de uma mensagem clara.
                if (string.IsNullOrWhiteSpace(entity.Nome))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Nome deve ser preenchido.";
                    return ret;
                }

                // Administrador de instituicao so cria conta na propria instituicao (nunca
                // sem instituicao, o que viraria admin da plataforma). O admin da plataforma
                // escolhe a instituicao; 0/nulo ("Administração geral") = sem instituicao.
                long? institutionId = IsPlatformAdmin
                    ? (entity.InstitutionId is > 0 ? entity.InstitutionId : null)
                    : _currentUser.InstitutionId;

                var existing = await _repo.FindBy(l => l.Email == entity.Email);

                if (existing.Any())
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Já existe uma conta com este email.";
                    return ret;
                }

                var hashed = BCrypt.Net.BCrypt.HashPassword(entity.Password);

                // Id e UserId com o mesmo valor ja no insert. Antes a conta era gravada
                // com UserId = 0 e depois copiava Id -> UserId, o que o EF recusa (UserId e
                // chave alternativa): a tela recebia erro e a conta ficava com UserId 0.
                var newId = await _repo.GetNextAccountId();

                var loginAccount = new LoginAccount
                {
                    Id = newId,
                    UserId = newId,
                    Name = entity.Nome.Trim(),
                    Email = entity.Email,
                    PasswordHash = hashed,

                    UserType = entity.UserType,
                    InstitutionId = institutionId,
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
                    Name = loginAccount.Name,
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
                if (existing == null || !CanView(existing))
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Conta não encontrada.";
                    return ret;
                }



                existing.Email = entity.Email;
                existing.Name = entity.Name;
                existing.LastLogin = entity.LastLogin;

                // Papel e status so podem ser alterados por quem administra a conta;
                // o proprio usuario (tela de perfil) mantem os valores atuais.
                if (CanManage(existing))
                {
                    existing.UserType = entity.UserType;
                    existing.Active = entity.Active ?? true;
                }


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
                if (existing == null || !CanManage(existing))
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
