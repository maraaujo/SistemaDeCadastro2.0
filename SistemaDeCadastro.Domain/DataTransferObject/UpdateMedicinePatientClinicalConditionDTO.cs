using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdateMedicinePatientClinicalConditionDTO
    {
        public long Id { get; set; }
        public long MedicineId { get; set; }
        public long PatientClinicalConditionId { get; set; }
        public string PrescribedDosage { get; set; }
        public string Frequency { get; set; }
        public TimeSpan? AdministrationTime { get; set; }
        public long? ResponsibleEmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Observations { get; set; }
    }
}
