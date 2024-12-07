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
    public class MedicineRepository : BaseRepository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(SistemaCadastroContext context)
            : base(context)
        {
        }

        public async Task<List<Medicine>> GetMedicineById(long ind)
        {
            return await this.FindBy( c => c.Id == ind );  
        }
        public async Task GetMedicineByAnyValorString(string medicine)
        {
            await this.Any( c => c.Name == medicine );
           
        }

        public async Task CreateMedicine(Medicine medicine)
        {
            await this.Create(medicine);
        }

        public async Task UpdateMedicine(Medicine medicine)
        {
            await this.Update(medicine);
        }
        public async Task DeleteMedicine(Medicine medicine)
        {
            await this.Delete(medicine);
        }
    }
}
