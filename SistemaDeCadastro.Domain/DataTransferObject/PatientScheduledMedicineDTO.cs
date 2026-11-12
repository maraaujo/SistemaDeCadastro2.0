using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    // Medicamento programado (prescricao) de um acolhido, com o id da propria
    // prescricao. A tela de edicao do acolhido precisa desse id para atualizar o
    // registro existente em vez de criar outro.
    public class PatientScheduledMedicineDTO
    {
        public long Id { get; set; }

        public long MedicineId { get; set; }

        public string? MedicineName { get; set; }

        public long PatientClinicalConditionId { get; set; }

        public long ClinicalConditionId { get; set; }

        public string? ClinicalConditionName { get; set; }

        public string? PrescribedDosage { get; set; }

        public string? Frequency { get; set; }

        public TimeSpan? AdministrationTime { get; set; }

        public long? ResponsibleEmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Observations { get; set; }
    }
}
