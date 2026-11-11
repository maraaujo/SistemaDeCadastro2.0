using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface ILoginAccountApp
    {
        /// <summary>Devolve as contas de acesso que o usuario logado pode ver (sem senha).</summary>
        Task<List<LoginAccountDTO>> GetAll();
        /// <summary>Devolve a conta do Id informado se o usuario puder ve-la; senao null.</summary>
        Task<LoginAccountDTO?> GetById(long id);
        /// <summary>Cria uma conta de acesso (somente administradores).</summary>
        Task<ApiResponse> Create(CreateLoginAccountDTO entity);
        /// <summary>Atualiza uma conta de acesso; papel e status so mudam para quem administra a conta.</summary>
        Task<ApiResponse> Update(UpdateLoginAccountDTO entity);
        /// <summary>Exclui uma conta de acesso (somente quem administra a conta).</summary>
        Task<ApiResponse> Delete(long id);
    }
}
