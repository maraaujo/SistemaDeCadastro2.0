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
       /// <summary>Devolve uma lista com o vinculo medicamento-doenca do acolhido do Id informado.</summary>
       Task<List<MedicinePatientIllness>> GetMedicinePatientIllnessesById(long id);
       /// <summary>Atualiza um vinculo medicamento-doenca do acolhido.</summary>
       Task<ApiResponse> UpdateMedicinePatientIllness(MedicinePatientIllnessDTO medicinePatientIllness);
        /// <summary>Cria um vinculo medicamento-doenca do acolhido.</summary>
        Task<ApiResponse> CreateMedicinePatientIllness(MedicinePatientIllnessDTO medicinePatientIllness);

    }
}
