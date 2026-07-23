using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IClinicalConditionRepository : IBaseRepository<ClinicalCondition>
    {
        Task<ClinicalCondition?> GetById(long id);
        Task<PagedClinicalConditionDTO> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter);
    }
}
