
using Azure;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Model;
using System.Runtime.InteropServices;
using SistemaDeCadastro.Infra.Interface;
namespace SistemaDeCadastro.APP
{
    public class IdosoApp : IIdosoApp
    {
        private readonly IIdosoRepository _idosoRepository;
        private readonly IFuncionarioRepository _funcionarioRepository;
        private readonly IIdosoDoencaRepository _idosoDoencaRepository;
    

        public IdosoApp(IIdosoRepository idosoRepository, IAdminRepository adminRepository)
        {
            _idosoRepository = idosoRepository;
           
        }

       
      
        
        public async Task GetIdososComecandoComLetraAEDoecnaComLetraP()
        {
            var ret = (await _idosoDoencaRepository.FindBy(c => c.Idosos.Nome.StartsWith("a")
            && c.Doencas.Nome.StartsWith("p"))).Select(c => new
            {
                Nome = c.Idosos.Nome,
                Doenca = c.Doencas.Nome
            }).ToList();
        }

        public async Task Update(Idoso idoso) => await this._idosoRepository.Update(idoso);
        public async Task Delete(Idoso idoso) => await this._idosoRepository.Delete(idoso);

        
       


    }
}
