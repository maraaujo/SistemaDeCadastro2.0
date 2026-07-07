using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class MedicinePatientClinicalCondition
    {
        public long Id { get; set; }

        public long MedicineId { get; set; }

        public long PatientClinicalConditionId { get; set; }

        public string PrescribedDosage { get; set; }

        public string Frequency { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Observations { get; set; }

        public virtual Medicine Medicine { get; set; }

        public virtual PatientClinicalCondition PatientClinicalCondition { get; set; }
    }
}
