using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PatientClinicalConditionListDTO
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public long ClinicalConditionId { get; set; }
        public string ClinicalConditionName { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public string Observations { get; set; }
    }
}