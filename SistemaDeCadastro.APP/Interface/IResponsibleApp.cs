using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IResponsibleApp
    {
        Task<List<Responsible>> GetAll();
        Task<Responsible?> GetById(long id);
        Task<ApiResponse> Create(Responsible entity);
        Task<ApiResponse> Update(Responsible entity);
        Task<ApiResponse> Delete(long id);
    }
}
