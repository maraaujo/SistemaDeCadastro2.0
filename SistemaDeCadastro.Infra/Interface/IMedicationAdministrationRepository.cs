using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicationAdministrationRepository : IBaseRepository<MedicationAdministration>
    {
        /// <summary>Historico paginado de administracoes de medicamento com acolhido, funcionario e medicamento, aplicando os filtros.</summary>
        Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter);
        /// <summary>Busca a administracao de medicamento pelo id (lista com 0 ou 1 item).</summary>
        Task<List<MedicationAdministration>> GetById(long ind);
        /// <summary>Lista as administracoes de medicamento com o status exato informado.</summary>
        Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status);
        /// <summary>Lembretes dos medicamentos programados para hoje que ainda nao foram administrados, com tempo restante/atraso (fuso de Sao Paulo).</summary>
        Task<List<MedicineReminderDTO>> GetMedicineReminders();
        /// <summary>Relatorio do dia: todos os medicamentos programados para a data com status Administrado, Atrasado ou Pendente.</summary>
        Task<List<DailyMedicationReportItemDTO>> GetDailyMedicationReport(DateTime? referenceDate, long? patientId);
    }
}
