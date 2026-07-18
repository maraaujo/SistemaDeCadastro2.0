using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class PatientIllnessFilterDTO
    {
        public long Id { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public long? IllnessId { get; set; }
        public string IllnessName { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public int Page { get; set; } = 1;
    }
}