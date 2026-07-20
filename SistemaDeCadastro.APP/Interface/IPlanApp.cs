using SistemaDeCadastro.Domain.DataTransferObject;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.Models.Stage;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IPlanApp
    {
        Task<ApiResponse> Create(CreatePlanDTO entity);
        Task<ApiResponse> Update(UpdatePlanDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<ApiResponse> GetById(long id);
        Task<ApiResponse> GetAll();
    }
}
