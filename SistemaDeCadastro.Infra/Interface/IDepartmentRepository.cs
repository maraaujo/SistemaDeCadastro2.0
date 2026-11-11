using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        /// <summary>Busca um departamento pelo id (somente da instituicao do usuario logado).</summary>
        Task<Department?> GetById(long id);
        /// <summary>Lista paginada de departamentos filtrando por nome e descricao.</summary>
        Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter);
    }
}
