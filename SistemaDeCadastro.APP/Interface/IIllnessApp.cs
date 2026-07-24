using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;


namespace SistemaDeCadastro.APP.Interface
{
    public interface IIllnessApp
    {
        Task<ApiResponse> CreateIllness(CreateIllnessDTO illness);
        Task<List<Illness>> GetIllnessById(long id);
        Task GetAnyIllnessIfTheValor(string illness);
        Task<ApiResponse> UpdateIllness(UpdateIllnessDTO illness);
        Task<ApiResponse> DeleteIllness(IllnessDTO illness);
        Task<ApiResponse> GetAllIllness();
        Task<PagedIllnessDTO> GetIllnessByFilter(IllnessFilterDTO filter);
    }
}
