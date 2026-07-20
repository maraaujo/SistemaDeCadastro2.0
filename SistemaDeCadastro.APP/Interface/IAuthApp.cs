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
        Task<ApiResponse> Login(LoginRequestDTO login);

    }
}
