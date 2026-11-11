using SistemaDeCadastro.Domain.Models.Stage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IResponsibleRepository : IBaseRepository<Responsible>
    {
        /// <summary>Busca um responsavel (familiar/contato) pelo id.</summary>
        Task<Responsible?> GetById(long id);
        /// <summary>Lista os responsaveis de um acolhido.</summary>
        Task<List<Responsible>> GetByPatientId(long patientId);
    }
}
