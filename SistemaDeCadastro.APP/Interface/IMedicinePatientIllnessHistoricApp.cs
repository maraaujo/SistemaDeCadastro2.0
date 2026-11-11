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
        /// <summary>Atualiza um registro do historico de medicamento por doenca do acolhido.</summary>
        Task<ApiResponse> UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoricDTO medicinePatientIllnessHistoric);
        /// <summary>Cria um registro no historico de medicamento por doenca do acolhido.</summary>
        Task<ApiResponse> CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoricDTO medicinePatientIllnessHistoric);
        /// <summary>Devolve uma lista com o registro de historico do Id informado.</summary>
        Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id);
    }
}
