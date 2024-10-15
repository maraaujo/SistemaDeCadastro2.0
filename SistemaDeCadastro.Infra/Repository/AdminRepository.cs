using SistemaDeCadastro.Data.Interface;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
   
    public class AdminRepository : BaseRepository<Pessoa>, IAdminRepository
    {

    {
        private readonly SistemaDeCadastroContext context;
        private readonly IPessoaRepository pessoaRepository;
        private readonly IMedicamentoMorbidadeRepository medicamentoMorbidadeRepository;
        private readonly IMorbidadaRepository morbidadaRepository;
        private readonly IPosologiaRepository posologiaRepository;

        public AdminRepository(SistemaDeCadastroContext _context, IPessoaRepository _pessoaREpository,
         IMedicamentoMorbidadeRepository _medicamentoMorbidadeRepository,
         IMorbidadaRepository _motbidadeRepository,
         IPosologiaRepository posologiaRepository) : base(_context)
        {
        }

        //Vai herdar de todas os repositorios. Intuitoi?
        //Filtar as pesquisas 

        public async Task<PagedPessoaDTO> GetPessoa(PessoaDTO filter)
        {
            try
            {
                var iRet = this.context.Pessoas.AsQueryable();

                if (!string.IsNullOrEmpty(filter.Nome))
                    iRet = iRet.Where(c => c.Nome.ToLower().StartsWith(filter.Nome.ToLower()) || c.Nome.ToLower().EndsWith(filter.Nome.ToLower()) || c.Nome.ToLower() == filter.Nome.ToLower());

                if (!string.IsNullOrEmpty(filter.Cpf))
                    iRet = iRet.Where(c => c.Cpf.ToLower() == filter.Cpf.ToLower());

                if (!string.IsNullOrEmpty(filter.TipoSanguineo))
                    iRet = iRet.Where(c => c.TipoSanguineo.Contains(filter.TipoSanguineo));

                if (!string.IsNullOrEmpty(filter.Endereco))
                    iRet = iRet.Where(c => c.Endereco.ToLower() == filter.Endereco.ToLower());

                if (!string.IsNullOrEmpty(filter.DataObito))
                    iRet = iRet.Where(c => c.DataObito.ToLower() == filter.DataObito.ToLower());



            }
        }
    }
}
