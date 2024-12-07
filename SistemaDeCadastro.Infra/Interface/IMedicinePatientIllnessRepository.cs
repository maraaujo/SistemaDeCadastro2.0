using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicinePatientIllnessRepository
    {
        Task CreateMedicinePatientIllness(MedicinePatientIllness medicinePatientIllness);
        Task UpdateMedicinePatientIllness(MedicinePatientIllness medicinePatientIllness);
        Task<List<MedicinePatientIllness>> GetMedicinePatientIllnessesById(long id);
    }
}
