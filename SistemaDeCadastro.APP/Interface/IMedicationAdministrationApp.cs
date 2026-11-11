using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicationAdministrationApp
    {
        /// <summary>Devolve o historico paginado de administracoes de medicamento conforme o filtro.</summary>
        Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter);
        /// <summary>Devolve uma lista com a administracao de medicamento do Id informado.</summary>
        Task<List<MedicationAdministration>> GetById(long ind);
        /// <summary>Devolve as administracoes de medicamento com o status informado.</summary>
        Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status);
        /// <summary>Registra a administracao de um medicamento programado (bloqueia duplicado no mesmo horario).</summary>
        Task<ApiResponse> Create(CreateMedicationAdministrationDTO medicationAdministration);
        /// <summary>Atualiza uma administracao de medicamento existente.</summary>
        Task<ApiResponse> Update(UpdateMedicationAdministrationDTO medicationAdministration);
        /// <summary>Exclui a administracao de medicamento do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve os lembretes das proximas doses de medicamentos programados.</summary>
        Task<List<MedicineReminderDTO>> GetMedicineReminders();
        /// <summary>Devolve o relatorio diario de medicacao da data (e opcionalmente de um acolhido).</summary>
        Task<List<DailyMedicationReportItemDTO>> GetDailyMedicationReport(DateTime? referenceDate, long? patientId);

    }
}
