using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdatePatientClinicalConditionDTO
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public long ClinicalConditionId { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public string Observations { get; set; }
    }
}
