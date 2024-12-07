using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicinePatientIllnessHistoricDTO
    {
        public long Id { get; set; }

        public long IdMedicinePatientIllness { get; set; }
    }
}
