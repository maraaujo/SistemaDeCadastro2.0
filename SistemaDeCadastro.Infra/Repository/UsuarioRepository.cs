using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.Infra.Repository
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {

        private IMapper _mapper;

        private UserManager<Usuario> _userManager;
        private SignInManager<Usuario> _signInManager;
        private ITokenServiceRepository _tokenService;

        public UsuarioRepository(IMapper mapper, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager,
            ITokenServiceRepository tokenService,
        SistemaDeCadastroContext context) : base(context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> Cadastrar(UsuarioDTO dto)
        {
            Usuario usuario = _mapper.Map<Usuario>(dto);

            IdentityResult resultado = await _userManager.CreateAsync(usuario, dto.Password);

            if (!resultado.Succeeded)
            {
                throw new ApplicationException("Falha ao cadastrar usuário!");
            }
            return resultado;

        }

        public async Task<string> Login(LoginUsuarioDto dto)
        {
            try
            {
                var resultado = await this._signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);

                if (!resultado.Succeeded)
                {
                    Console.WriteLine($"Falha ao autenticar usuário: {dto.Username}");
                    throw new ApplicationException("Usuário não autenticado!");
                }

                var usuario = this._signInManager.UserManager
                    .Users
                    .FirstOrDefault(user => user.NormalizedUserName == dto.Username.ToUpper());

                if (usuario == null)
                {
                    Console.WriteLine($"Usuário não encontrado: {dto.Username}");
                    throw new ApplicationException("Usuário não encontrado!");
                }

                var token = this._tokenService.GenerationToken(usuario);
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro durante login: {ex.Message}");
                throw;
            }
        }

    }
}


