using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicinePatientIllnessDTO
    {
        public long Id { get; set; }

        public long IdPatientIllness { get; set; }

        public long IdMedicine { get; set; }

        public string Dosage { get; set; } = null!;

        public int Time { get; set; }
    }
}
