using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class IllnessRepository : BaseRepository<Illness>, IIllnessRepository
    {
        public IllnessRepository(SistemaCadastroContext context) 
            : base(context)
        {
        }

        public async Task<List<Illness>> GetIllnessById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }

        public async Task<List<Illness>> GetAllIllness()
        {
            return await this.GetAll(); 
        }

        public async Task DeleteIlless(Illness illness)
        {
            await this.Delete(illness);
        }
        public async Task CreateIllness(Illness illness) 
        {
            await this.Create(illness);
        }
        public async Task UpdateIllness(Illness illness)
        {
            await this.Update(illness);
        }

        public async Task GetAnyIllnessIfTheValor(string illness)
        {
            await this.Any(c => c.Name == illness);
        }
    }
}
