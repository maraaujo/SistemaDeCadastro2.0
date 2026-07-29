using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateMedicationAdministrationDTO
    {

        public long MedicinePatientClinicalConditionId { get; set; }

        public long PatientId { get; set; }

        public long? EmployeeId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? AdministeredDateTime { get; set; }

        public string Status { get; set; }

        public string Observations { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
