using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IEmployeeApp
    {
        /// <summary>Devolve todos os funcionarios da instituicao do usuario.</summary>
        Task<List<Employee>> GetAll();
        /// <summary>Devolve o funcionario do Id informado, ou null se nao existir.</summary>
        Task<Employee?> GetById(long id);
        /// <summary>Cadastra um funcionario na instituicao do usuario (exige departamento; valida CPF e telefone).</summary>
        Task<ApiResponse> Create(CreateEmployeeDTO entity);
        /// <summary>Atualiza os dados de um funcionario existente.</summary>
        Task<ApiResponse> Update(UpdateEmployeeDTO entity);
        /// <summary>Exclui o funcionario do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de funcionarios conforme o filtro.</summary>
        Task<PagedEmployeeDTO> GetEmployeeByFilter(EmployeeFilterDTO filter);
    }
}
