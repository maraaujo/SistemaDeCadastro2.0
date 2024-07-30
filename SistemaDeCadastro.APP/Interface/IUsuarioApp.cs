using Microsoft.AspNetCore.Identity;
using SistemaDeCadastro.Domain.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IUsuarioApp
    {
        Task<IdentityResult> Cadastrar(UsuarioDTO dto);
        Task<string> Login(LoginUsuarioDto dto);
    }
}
