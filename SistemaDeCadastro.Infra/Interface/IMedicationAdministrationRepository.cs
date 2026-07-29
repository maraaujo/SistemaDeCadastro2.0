using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicationAdministrationRepository : IBaseRepository<MedicationAdministration>
    {
        Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter);
        Task<List<MedicationAdministration>> GetById(long ind);
        Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status);
        Task Create(CreateMedicationAdministrationDTO medicationAdministration);
    }
}
