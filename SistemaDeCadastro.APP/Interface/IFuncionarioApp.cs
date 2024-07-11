using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.APP.Interface
{
    public interface IFuncionarioApp
    {
        Task<FuncionarioDTO> CreateFuncionario(FuncionarioDTO funcionariodto);
        Task<List<Funcionario>> GetFuncionairoById(int id);
       
    }
}
