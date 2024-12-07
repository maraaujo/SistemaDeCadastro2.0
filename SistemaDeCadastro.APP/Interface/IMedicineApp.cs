using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicineApp
    {
        Task<List<Medicine>> GetMedicineId(long id);
        Task<ApiResponse> CreateMedicine(Medicine medicine);
        Task<ApiResponse> UpdateMedicine(MedicineDTO medicine);
        Task<ApiResponse> DeleteMedicine(MedicineDTO medicine);
    }
}
