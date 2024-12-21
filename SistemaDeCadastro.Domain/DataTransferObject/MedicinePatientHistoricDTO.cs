using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicinePatientHistoricDTO
    {
        public long ID { get; set; }
        public DateTime LastTime { get; set; }
    }
}
