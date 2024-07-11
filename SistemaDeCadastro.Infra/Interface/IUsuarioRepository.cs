using Microsoft.AspNetCore.Identity;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IUsuarioRepository
    {
        Task<IdentityResult> Cadastrar(UsuarioDTO dto);
        Task<string> Login(LoginUsuarioDto dto);
    }
}
