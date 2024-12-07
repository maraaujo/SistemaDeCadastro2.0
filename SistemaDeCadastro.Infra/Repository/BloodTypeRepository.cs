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
    public class BloodTypeRepository : BaseRepository<BloodType>, IBloodTypeRepository
    {
        public BloodTypeRepository(SistemaCadastroContext context) 
            : base(context)
        {
        }

        public async Task<List<BloodType>> GetBloodTypeById(long id)
        {
           return await this.FindBy(c => c.Id == id);
            
        }
        public async Task<List<BloodType>> FindAll() 
        {
            return await this.GetAll();
        }

        public async Task<BloodType> CreateBooldType(BloodType booldtype)
        {
            await this.Create(booldtype); 
            return booldtype;
        }

        public async Task DeleteBloodtype(BloodType id) 
        {
            await this.Delete(id);
        }
    }
}
