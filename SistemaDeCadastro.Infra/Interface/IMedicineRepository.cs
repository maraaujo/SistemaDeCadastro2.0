using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicineRepository
    {
        Task<List<Medicine>> GetMedicineById(long ind);
        Task GetMedicineByAnyValorString(string medicine);
        Task CreateMedicine(Medicine medicine);
        Task UpdateMedicine(Medicine medicine);
        Task DeleteMedicine(Medicine medicine);
    }
}
