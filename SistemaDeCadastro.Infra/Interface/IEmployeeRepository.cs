using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        /// <summary>Busca um funcionario pelo id (somente da instituicao do usuario logado).</summary>
        Task<Employee?> GetById(long id);
        /// <summary>Lista os funcionarios de um departamento.</summary>
        Task<List<Employee>> GetByDepartmentId(long departmentId);
        /// <summary>Lista paginada de funcionarios com o nome do departamento, aplicando os filtros informados.</summary>
        Task<PagedEmployeeDTO> GetEmployeeByFilter(EmployeeFilterDTO filter);
        /// <summary>Diz se o funcionario ainda e usado por vinculos com acolhidos, medicamentos programados ou administracoes.</summary>
        Task<bool> HasDependencies(long employeeId);
    }
}

