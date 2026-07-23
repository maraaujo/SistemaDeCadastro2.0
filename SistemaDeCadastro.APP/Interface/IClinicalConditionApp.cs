using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IClinicalConditionApp
    {
        Task<List<ClinicalCondition>> GetAll();
        Task<ClinicalCondition?> GetById(long id);
        Task<ApiResponse> Create(CreateClinicalConditionDTO entity);
        Task<ApiResponse> Update(UpdateClinicalConditionDTO entity);
        Task<ApiResponse> Delete(long id);
        Task<PagedClinicalConditionDTO> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter);
    }
}
