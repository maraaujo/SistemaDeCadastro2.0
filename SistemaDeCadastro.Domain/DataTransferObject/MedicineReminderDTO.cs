using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicineReminderDTO
    {
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public TimeSpan? AdministrationTime { get; set; }
        public DateTime NextDoseDateTime { get; set; }
        public string ResponsibleEmployeeName { get; set; }
        public int MinutesRemaining { get; set; }
        public string AlertText { get; set; } = string.Empty;
    }
}
