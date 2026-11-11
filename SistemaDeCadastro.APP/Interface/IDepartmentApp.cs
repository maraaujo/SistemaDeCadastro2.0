using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IDepartmentApp
    {
        /// <summary>Devolve todos os departamentos da instituicao do usuario.</summary>
        Task<List<Department>> GetAll();
        /// <summary>Devolve o departamento do Id informado, ou null se nao existir.</summary>
        Task<Department?> GetById(long id);
        /// <summary>Cria um departamento na instituicao do usuario logado.</summary>
        Task<ApiResponse> Create(CreateDepartmentDTO entity);
        /// <summary>Atualiza nome e descricao de um departamento existente.</summary>
        Task<ApiResponse> Update(UpdateDepartmentDTO  entity);
        /// <summary>Exclui o departamento do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de departamentos conforme o filtro.</summary>
        Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter);
      
    }
}
