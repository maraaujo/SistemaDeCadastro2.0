using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IBloodTypeRepository : IBaseRepository<BloodType>
    {
        Task<List<BloodType>> GetBloodTypeById(long id);
        Task<List<BloodType>> FindAll();
        Task<BloodType> CreateBooldType(BloodType booldtype);
        Task DeleteBloodtype(BloodType id); 
    }
}
