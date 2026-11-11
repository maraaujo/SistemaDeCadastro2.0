using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    // Interface kept as-is (no changes)
    public interface IAuthApp
    {
        /// <summary>Faz o login por e-mail e senha e, se der certo, devolve os dados do usuario e o token JWT.</summary>
        Task<ApiResponse> Login(LoginRequestDTO login);

        // Confere, a cada requisicao, se a conta do token ainda existe, esta ativa e
        // mantem o mesmo papel e instituicao com que o token foi emitido.
        Task<bool> IsSessionValid(System.Security.Claims.ClaimsPrincipal principal);

    }
}
