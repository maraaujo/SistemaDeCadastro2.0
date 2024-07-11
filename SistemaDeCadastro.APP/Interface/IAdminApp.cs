
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Model;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAdminApp
    {
        Task<PagedIdosoDTO> GetIdoso(IdosoFilterDTO filter);
        Task<List<Idoso>> GetIdosoById(int id);
        Task<ApiResponseDTO> Create(IdosoDTO idosoDTO);
        Task<List<Idoso>> GetAllIdoso(); 
        Task Update(Idoso idoso);
        Task Delete(Idoso idoso);
        Task<List<ElderlyMedicineDTO>> GetElderlyMedicine(int id);
        Task<ApiResponseDTO> ValidateFuncionario(string email, string senha);



    }
}
