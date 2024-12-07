using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicinePatientIllnessHistoricRepository
    {
        Task CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric);
        Task UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric);
        Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id);
    }
}
