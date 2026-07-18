using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class PatientClinicalConditionFilterDTO
    {
        public long Id { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public long? ClinicalConditionId { get; set; }
        public string ClinicalConditionName { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public int Page { get; set; } = 1;
    }
}