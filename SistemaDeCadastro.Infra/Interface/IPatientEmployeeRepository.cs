using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IPatientEmployeeRepository : IBaseRepository<PatientEmployee>
    {
        /// <summary>Busca o vinculo acolhido x funcionario responsavel pelo id.</summary>
        Task<PatientEmployee?> GetById(long id);
        /// <summary>Lista os funcionarios vinculados a um acolhido.</summary>
        Task<List<PatientEmployee>> GetByPatientId(long patientId);
        /// <summary>Lista paginada dos vinculos acolhido x funcionario com nomes e cargo, aplicando os filtros.</summary>
        Task<PagedPatientEmployeeDTO> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter);
    }
}
