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
        public long MedicinePatientClinicalConditionId { get; set; }
        public string Frequency { get; set; }
        public TimeSpan? AdministrationTime { get; set; }
        public string ResponsibleEmployeeName { get; set; }
        // Horario previsto da proxima dose COM fuso explicito (-03:00). Enviado assim para
        // que o front calcule o instante correto, sem depender de assumir o fuso do servidor.
        public DateTimeOffset NextDoseDateTime { get; set; }
        public int MinutesRemaining { get; set; }
        public string AlertText { get; set; } = string.Empty;
    }
}
