using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicinePatientIllnessHistoricApp
    {
        Task<ApiResponse> UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoricDTO medicinePatientIllnessHistoric);
        Task<ApiResponse> CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoricDTO medicinePatientIllnessHistoric);
        Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id);
    }
}
