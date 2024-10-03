using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Domain.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PessoaRepository : BaseRepository<Pessoa>
    {
        public PessoaRepository(SistemaDeCadastroContext context) : base(context)
        {

        }

        public async Task< List<Pessoa>> GetAllPessoas()
        {
            try
            {
                var ret = await this.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar informações: {ex.Message}");
            }
        }
        public async Task<List<Pessoa>> GetPessoaById(int id)
        {
            return await this.FindBy(c => c.Cod == id);
        }

        public async Task UpdatePessoa(Pessoa newPessoa)
        {
            try
            {
                await this.Update(newPessoa);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar informações : {ex.Message}");
            }
        }

        public async Task DeletePessoa(Pessoa id)
        {
            try
            {
                await this.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar informações : {ex.Message}");
            }
        }
    }
}
