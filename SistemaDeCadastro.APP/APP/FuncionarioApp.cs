using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.APP
{
    public class FuncionarioApp : IFuncionarioApp
    {
        private readonly IFuncionarioRepository _funcionariorepository;
        public FuncionarioApp(IFuncionarioRepository funcionariorepository)
        {
            _funcionariorepository = funcionariorepository;
        }
        public async Task<FuncionarioDTO> CreateFuncionario(FuncionarioDTO funcionariodto)
        {
            await this._funcionariorepository.CreateFuncionario(funcionariodto);
            return funcionariodto;
        }
        public async Task<List<Funcionario>> GetFuncionairoById(int id)
         {
            var ret = await this._funcionariorepository.GetFuncionairoById(id);
            return ret;
        }

    }
}
