using SistemaDeCadastro.Domain.Context;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.Infra.Repository
{
    public class DepartamentoRepository : BaseRepository<Departamento>, IDepartamentoRepository
    {
        public SistemaDeCadastroContext context;
        public DepartamentoRepository(SistemaDeCadastroContext _context)
            : base(_context)
        {
            _context = context;
        }

        public async Task<List<Departamento>> GetDepartamentoById(int id)
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
        public async Task<DepartamentoDTO> CreateDepartamento(DepartamentoDTO departamentoDto)
        {
            try
            {
                Departamento departamento = new Departamento()
                {
                    Nome = departamentoDto.Nome,
                };

                await this.Create(departamento);
                
            }
            catch (Exception err)
            {
                throw new Exception($"Erro ao colocar informações do departamento");
            }
            return departamentoDto;
        }
    }
}
