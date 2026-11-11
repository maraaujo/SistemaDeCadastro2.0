using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IClinicalConditionRepository : IBaseRepository<ClinicalCondition>
    {
        /// <summary>Busca uma condicao clinica (catalogo) pelo id.</summary>
        Task<ClinicalCondition?> GetById(long id);
        /// <summary>Lista paginada de condicoes clinicas filtrando por nome, descricao e tipo.</summary>
        Task<PagedClinicalConditionDTO> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter);
    }
}
