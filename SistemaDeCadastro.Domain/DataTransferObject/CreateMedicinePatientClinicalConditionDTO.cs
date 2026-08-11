using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateMedicinePatientClinicalConditionDTO
    {
        public long PatientId { get; set; }

        public MedicineDTO MedicineDTO { get; set; }

        public ClinicalConditionDTO ClinicalConditionDTO { get; set; }
        public PatientClinicalConditionDTO PatientClinicalConditionDTO { get; set; }

        public long? ResponsibleEmployeeId { get; set; }

        public string PrescribedDosage { get; set; }

        public string Frequency { get; set; }

        public TimeSpan AdministrationTime { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Observations { get; set; }
    }
}
