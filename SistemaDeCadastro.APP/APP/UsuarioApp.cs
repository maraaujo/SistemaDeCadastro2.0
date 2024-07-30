using Microsoft.AspNetCore.Identity;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class UsuarioApp : IUsuarioApp
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioApp(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public async Task<IdentityResult> Cadastrar(UsuarioDTO dto)
        {
          var ret = await this._usuarioRepository.Cadastrar(dto);
            return ret;
           
        } 
        public async Task<string> Login(LoginUsuarioDto dto)
        {
           return await this._usuarioRepository.Login(dto);

        }

    }
}

