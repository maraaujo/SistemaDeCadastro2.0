using SistemaDeCadastro.Domain.Models.Stage;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IClinicalConditionApp
    {
        Task<List<ClinicalCondition>> GetAll();
        Task<ClinicalCondition?> GetById(long id);
        Task<ApiResponse> Create(ClinicalCondition entity);
        Task<ApiResponse> Update(ClinicalCondition entity);
        Task<ApiResponse> Delete(long id);
    }
}
