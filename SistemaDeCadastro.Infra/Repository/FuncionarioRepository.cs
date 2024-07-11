using AutoMapper;
using Azure;
using Microsoft.Extensions.Logging;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.Infra.Repository
{
    public class FuncionarioRepository : BaseRepository<Funcionario>, IFuncionarioRepository
    {
        private IMapper _mapper;

        public SistemaDeCadastroContext context;
        public FuncionarioRepository(SistemaDeCadastroContext _context, IMapper mapper)
            : base(_context)
        {
            
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<Funcionario>> GetFuncionairoByEmail(string email)
        {
            try
            {

               var ret = await this.FindBy(c => c.Email == email.ToLower());
                return ret.ToList();

            }
            catch (Exception err)
            {
                throw new Exception($"Erro ao recuperar informações do Funcionario: {err.Message}");
            }
        }
        public async Task<List<Funcionario>> GetFuncionairoById(int id)
        {
            try
            {
                
                return await this.FindBy(c => c.Id == id);

            }
            catch (Exception err)
            {
                throw new Exception($"Erro ao recuperar informações do Funcionario: {err.Message}");
            }
        }
        public async Task<FuncionarioDTO> CreateFuncionario(FuncionarioDTO funcionariodto)
        {
            try
            {
                //mapeando Dto para modelo 
                Funcionario funcionario = new Funcionario
                {
                    Nome = funcionariodto.Nome,
                    Sobrenome = funcionariodto.Sobrenome,
                    Documento = funcionariodto.Documento,
                    Email = funcionariodto.Email,
                    Senha = funcionariodto.Senha,
                    DepartamentoId =funcionariodto.DepartamentoId,

                };

               await this.Create(funcionario);
                
            }
            catch (Exception err)
            {
                throw new Exception($"Erro ao colocar informações do Funcionario: {err.Message}");
            }
            return funcionariodto;
        }
    }
}
