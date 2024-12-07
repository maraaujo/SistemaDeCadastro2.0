using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicinePatientIllnessApp
    {
       Task<List<MedicinePatientIllness>> GetMedicinePatientIllnessesById(long id);
       Task<ApiResponse> UpdateMedicinePatientIllness(MedicinePatientIllnessDTO medicinePatientIllness);
        Task<ApiResponse> CreateMedicinePatientIllness(MedicinePatientIllnessDTO medicinePatientIllness);

    }
}
