using Microsoft.AspNetCore.Identity;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class PessoaApp : IPessoaApp
    {
        private readonly IPessoaRepository pessoaRepository;

        public PessoaApp(IPessoaRepository _pessoaRepository)
        {
           
             this.pessoaRepository = _pessoaRepository;
        }
        public async Task<List<Pessoa>> GetAllPessoas() =>  await this.pessoaRepository.GetAllPessoas();
        public async Task<List<Pessoa>> GetPessoaById(int id) => await this._pessoaRepository.GetPessoaById(int id);
    }
}
