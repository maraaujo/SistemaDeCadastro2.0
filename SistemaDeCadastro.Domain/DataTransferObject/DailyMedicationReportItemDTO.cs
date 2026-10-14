using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    // Uma linha do "relatorio de medicamentos do dia": um medicamento programado para a
    // data de referencia, com o horario previsto e a situacao (Administrado / Atrasado /
    // Pendente) calculada em relacao ao horario atual de Sao Paulo.
    public class DailyMedicationReportItemDTO
    {
        public long MedicinePatientClinicalConditionId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string ResponsibleEmployeeName { get; set; }

        // Horario de parede previsto para hoje (com fuso explicito -03:00).
        public TimeSpan? AdministrationTime { get; set; }
        public DateTimeOffset ScheduledDateTime { get; set; }

        // Quando foi efetivamente administrado (null se ainda nao foi), com fuso explicito.
        public DateTimeOffset? AdministeredDateTime { get; set; }

        // "Administrado", "Atrasado" ou "Pendente".
        public string Status { get; set; }

        public string Observations { get; set; }
    }
}
