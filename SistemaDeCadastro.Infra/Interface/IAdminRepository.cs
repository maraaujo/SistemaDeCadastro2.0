using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;

namespace SistemaDeCadastro.Data.Interface
{
    public interface IAdminRepository
    {
        Task<PagedIdosoDTO> GetIdoso(IdosoFilterDTO filter);
        Task<List<Idoso>> GetIdosoById(int id);
        Task<List<Idoso>> GetAllIdoso();
        Task<List<ElderlyMedicineDTO>> GetElderlyMedicine(int id);
        Task<Funcionario> ValidateFuncionario(string email, string senha);
    }
}
