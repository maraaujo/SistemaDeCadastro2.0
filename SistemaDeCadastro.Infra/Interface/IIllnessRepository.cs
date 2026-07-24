using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;


namespace SistemaDeCadastro.Infra.Interface
{
    public interface IIllnessRepository : IBaseRepository<Illness>
    {
        Task<List<Illness>> GetIllnessById(long id);
        Task<List<Illness>> GetAllIllness();
        Task DeleteIlless(Illness illness);
        Task CreateIllness(Illness illness);
        Task UpdateIllness(Illness illness);
        Task GetAnyIllnessIfTheValor(string illness);
        Task<PagedIllnessDTO> GetIllnessByFilter(IllnessFilterDTO filter);
    }
}
