using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicinePatientClinicalConditionApp
    {
        /// <summary>Devolve todos os medicamentos programados da instituicao.</summary>
        Task<List<MedicinePatientClinicalCondition>> GetAll();
        /// <summary>Devolve o medicamento programado do Id informado, ou null se nao existir.</summary>
        Task<MedicinePatientClinicalCondition?> GetById(long id);
        /// <summary>Programa um medicamento para um acolhido (bloqueia o mesmo medicamento no mesmo horario).</summary>
        Task<ApiResponse> Create(CreateMedicinePatientClinicalConditionDTO entity);
        /// <summary>Atualiza um medicamento programado existente.</summary>
        Task<ApiResponse> Update(UpdateMedicinePatientClinicalConditionDTO entity);
        /// <summary>Exclui o medicamento programado do Id informado.</summary>
        Task<ApiResponse> Delete(long id);

        /// <summary>Devolve os medicamentos programados de um vinculo acolhido + condicao clinica.</summary>
        Task<List<MedicinePatientClinicalCondition>> GetByPatientClinicalConditionId(long patientClinicalConditionId);
      
        /// <summary>Devolve a lista paginada de medicamentos programados conforme o filtro.</summary>
        Task<PagedMedicinePatientClinicalConditionDTO> GetMedicinePatientClinicalConditionByFilter(MedicinePatientClinicalConditionFilterDTO filter);
    }
}
