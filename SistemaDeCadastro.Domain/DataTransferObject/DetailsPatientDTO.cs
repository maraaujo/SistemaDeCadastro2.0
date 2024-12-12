using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class DetailsPatientDTO
    {
        public string Name { get; set; }
        public string IllnessName { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public int Time { get; set; }
        public DateTime LastTime { get; set; }

    }
}
