using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.Interface
{
    public interface IDepartamentoApp
    {
        Task<DepartamentoDTO> CreateDepartamento(DepartamentoDTO departamentoDto);
        Task<List<Departamento>> GetDepartamentoById(int id);
    }
}
