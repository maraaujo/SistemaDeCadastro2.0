using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicinePatientClinicalConditionRepository : IBaseRepository<MedicinePatientClinicalCondition>
    {
        /// <summary>Busca um medicamento programado pelo id, carregando medicamento, funcionario responsavel, acolhido e condicao clinica.</summary>
        Task<MedicinePatientClinicalCondition?> GetById(long id);
        /// <summary>Lista os medicamentos programados de uma condicao clinica do acolhido, com medicamento, responsavel e acolhido.</summary>
        Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId);
        /// <summary>Lista paginada de medicamentos programados com nomes de acolhido, medicamento, condicao e responsavel, aplicando os filtros.</summary>
        Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter);

        // Remove TODOS os medicamentos programados de TODAS as instituicoes
        // (ignora o filtro de tenant). Uso exclusivo do administrador da plataforma.
        Task<int> DeleteAllGlobal();
        Task LogGlobalCleanup(long userId, string? ipAddress, int scheduledRemoved, int administrationsRemoved);
    }
}
