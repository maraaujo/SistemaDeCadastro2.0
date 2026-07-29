using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class MedicationAdministration
    {
        public long Id { get; set; }

        public long MedicinePatientClinicalConditionId { get; set; }

        public long PatientId { get; set; }

        public long? EmployeeId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? AdministeredDateTime { get; set; }

        public string Status { get; set; }

        public string Observations { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual MedicinePatientClinicalCondition MedicinePatientClinicalCondition { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
