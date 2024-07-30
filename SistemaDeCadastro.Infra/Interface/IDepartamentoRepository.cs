using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.Infra.Interface
{
    public interface IDepartamentoRepository
    {
        Task<DepartamentoDTO> CreateDepartamento(DepartamentoDTO departamentoDto);
        Task<List<Departamento>> GetDepartamentoById(int id);
    }
}
