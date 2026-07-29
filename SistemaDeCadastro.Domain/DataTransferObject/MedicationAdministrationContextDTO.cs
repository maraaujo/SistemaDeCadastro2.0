using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicationAdministrationContextDTO
    {
        public string PatientName { get; set; }

        public string MedicineName { get; set; }

        public string PrescribedDosage { get; set; }

        public string Frequency { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? AdministeredDateTime { get; set; }

        public string Status { get; set; }

        public string EmployeeName { get; set; }

        public string Observations { get; set; }
    }
}
