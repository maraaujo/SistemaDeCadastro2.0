using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicinePatientIllnessHistoricRepository: IBaseRepository<MedicinePatientIllnessHistoric>
    {
        /// <summary>Grava um novo registro de historico de medicamento/doenca do acolhido.</summary>
        Task CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric);
        /// <summary>Atualiza um registro de historico de medicamento/doenca do acolhido.</summary>
        Task UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric);
        /// <summary>Busca o registro de historico pelo id (lista com 0 ou 1 item).</summary>
        Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id);
    }
}
