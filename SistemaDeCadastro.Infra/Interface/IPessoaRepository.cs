using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        Task<List<Pessoa>> GetAllPessoas();
        Task<List<Pessoa>> GetPessoaById(int id);
        Task UpdatePessoa(Pessoa newPessoa);
        Task DeletePessoa(Pessoa id); 
    }
}
