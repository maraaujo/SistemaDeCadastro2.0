using SistemaDeCadastro.Domain.Models.Stage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IClinicalConditionRepository : IBaseRepository<ClinicalCondition>
    {
        Task<ClinicalCondition?> GetById(long id);
    }
}
