using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IIllnessRepository
    {
        Task<List<Illness>> GetIllnessById(long id);
        Task<List<Illness>> GetAllIllness();
        Task DeleteIlless(Illness illness);
        Task CreateIllness(Illness illness);
        Task UpdateIllness(Illness illness);
        Task GetAnyIllnessIfTheValor(string illness);
    }
}
