using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicationAdministrationDTO
    {
        public long Id { get; set; }

        public long MedicinePatientClinicalConditionId { get; set; }

        public long PatientId { get; set; }
        public string NamePatient { get; set; }
        public string EmployeeName { get; set; }
        public long? EmployeeId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? AdministeredDateTime { get; set; }

        public string Status { get; set; }

        public string Observations { get; set; }

        public DateTime CreatedAt { get; set; }

        // Horario de parede planejado para a dose (vem do medicamento programado).
        // Usado na tela de historico para exibir o horario previsto quando o
        // ScheduledDateTime de registros antigos veio zerado (00:00).
        public TimeSpan? AdministrationTime { get; set; }

        // Horario da proxima medicacao deste item (hoje + AdministrationTime), com fuso
        // explicito (-03:00). Exibido na mesma tela de historico.
        public DateTimeOffset? NextDoseDateTime { get; set; }
    }
}
