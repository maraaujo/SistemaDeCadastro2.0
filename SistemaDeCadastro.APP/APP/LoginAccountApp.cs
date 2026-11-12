using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Validators;
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

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de contas de acesso e o usuario logado.
        /// </summary>
        public LoginAccountApp(ILoginAccountRepository repo, ICurrentUserServiceContext currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        /// <summary>
        /// True se o usuario logado tem o papel "Administrador" (com ou sem instituicao).
        /// </summary>
        private bool IsAdmin =>
            string.Equals(_currentUser.Role, AdminRole, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// True se o usuario logado e administrador da plataforma (Administrador sem instituicao).
        /// </summary>
        private bool IsPlatformAdmin => _currentUser.IsPlatformAdmin;

        /// <summary>
        /// True se o usuario logado e administrador de uma instituicao (Administrador com InstitutionId).
        /// </summary>
        private bool IsInstitutionAdmin => IsAdmin && _currentUser.InstitutionId.HasValue;

        /// <summary>
        /// True se a conta informada pertence ao proprio usuario logado (mesmo UserId).
        /// </summary>
        private bool IsOwnAccount(LoginAccount account) =>
            _currentUser.UserId.HasValue && account.UserId == _currentUser.UserId.Value;

        // Pode administrar a conta (alterar papel/status, apagar).
        private bool CanManage(LoginAccount account) =>
            IsPlatformAdmin ||
            (IsInstitutionAdmin && account.InstitutionId == _currentUser.InstitutionId);

        /// <summary>
        /// Pode ver/editar dados basicos da conta: quem a administra ou o proprio dono da conta.
        /// </summary>
        private bool CanView(LoginAccount account) => CanManage(account) || IsOwnAccount(account);

        /// <summary>
        /// Lista as contas de acesso conforme o papel: admin da plataforma ve todas, admin de instituicao ve
        /// as da propria instituicao e os demais veem so a propria conta. Nunca devolve o hash da senha.
        /// </summary>
        public async Task<List<LoginAccountDTO>> GetAll()
        {
            List<LoginAccount> accounts;

            if (IsPlatformAdmin)
            {
                accounts = await _repo.GetAllReadOnly();
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

        /// <summary>
        /// Busca uma conta pelo Id e devolve o DTO sem senha; devolve null se nao existir
        /// ou se o usuario logado nao puder ve-la (nao administra a conta e nao e o dono).
        /// </summary>
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

        /// <summary>
        /// Cria uma conta de acesso. So administradores podem criar; exige e-mail, senha forte (PasswordValidator)
        /// e nome, e recusa e-mail ja cadastrado. Admin de instituicao cria sempre na propria instituicao;
        /// admin da plataforma escolhe a instituicao (0/nulo = sem instituicao). A senha e gravada com hash BCrypt,
        /// Id e UserId recebem o mesmo valor e a conta nasce ativa. Devolve em Data os dados da conta sem a senha.
        /// </summary>
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

                if (!PasswordValidator.IsValid(entity.Password))
                {
                    ret.Success = false;
                    ret.ErrorMessage = PasswordValidator.MensagemInvalido;
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }

            return ret;
        }
        /// <summary>
        /// Atualiza e-mail e nome de uma conta que o usuario pode ver ("Conta nao encontrada." caso contrario); campo nao
        /// enviado mantem o valor atual. Papel (UserType), status (Active) e nova senha so mudam se o usuario logado
        /// administra a conta. Recusa e-mail que ja pertence a outra conta. Devolve o DTO atualizado (sem senha) em Data.
        /// </summary>
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



                // So altera o que foi enviado: campo ausente (nulo/vazio) mantem o valor
                // atual. Antes, um envio sem nome gravava nome nulo, um envio sem
                // "active" reativava a conta e o ultimo login era apagado.
                if (!string.IsNullOrWhiteSpace(entity.Email) &&
                    !string.Equals(entity.Email.Trim(), existing.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var newEmail = entity.Email.Trim();
                    if (await _repo.Any(l => l.Email == newEmail && l.Id != existing.Id))
                    {
                        ret.Success = false;
                        ret.ErrorMessage = "Já existe uma conta com este email.";
                        return ret;
                    }
                    existing.Email = newEmail;
                }

                if (!string.IsNullOrWhiteSpace(entity.Name))
                    existing.Name = entity.Name.Trim();

                // O ultimo login e gravado pelo proprio login; nao vem do cliente.

                // Papel, status e senha so podem ser alterados por quem administra a
                // conta; o proprio usuario (tela de perfil) mantem os valores atuais.
                if (CanManage(existing))
                {
                    if (!string.IsNullOrWhiteSpace(entity.UserType))
                        existing.UserType = entity.UserType;

                    if (entity.Active.HasValue)
                        existing.Active = entity.Active.Value;

                    // "Nova senha" da tela de contas: so troca se foi preenchida.
                    if (!string.IsNullOrWhiteSpace(entity.Password))
                    {
                        if (!PasswordValidator.IsValid(entity.Password))
                        {
                            ret.Success = false;
                            ret.ErrorMessage = PasswordValidator.MensagemInvalido;
                            return ret;
                        }
                        existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.Password);
                    }
                }


                await _repo.Update(existing);

                ret.Success = true;
                ret.Message = "Usuário atualizado com sucesso.";
                ret.Data = ToDTO(existing);
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Exclui uma conta de acesso. So quem administra a conta (admin da plataforma ou admin da mesma instituicao)
        /// pode excluir; nos demais casos, ou se nao existir, retorna "Conta nao encontrada.".
        /// </summary>
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }
    }
}
