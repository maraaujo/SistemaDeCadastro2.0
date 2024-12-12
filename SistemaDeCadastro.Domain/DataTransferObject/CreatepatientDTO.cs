using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreatepatientDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Responsible { get; set; }
        public string Phone { get; set; }
        public int BooldType { get; set; }
        public  List<MedicinePatientIllnessDTO> MedicinePatientIllnesses { get; set; }

    }
}
