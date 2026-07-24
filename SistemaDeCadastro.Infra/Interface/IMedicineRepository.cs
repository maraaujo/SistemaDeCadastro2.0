using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;


namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicineRepository : IBaseRepository<Medicine>
    {
        Task<List<Medicine>> GetMedicineById(long ind);
        Task GetMedicineByAnyValorString(string medicine);
        Task CreateMedicine(Medicine medicine);
        Task UpdateMedicine(Medicine medicine);
        Task DeleteMedicine(Medicine medicine);
        Task<PagedMedicineDTO> GetMedicineByFilter(MedicineFilterDTO filter);
    }
}
