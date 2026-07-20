using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IInstitutionApp
    {
        Task<ApiResponse> Create(CreateInstitutionDTO entity);
        Task<ApiResponse> Update(UpdateInstitutionDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<ApiResponse> GetById(long id);
        Task<ApiResponse> GetAll();
    }
}
