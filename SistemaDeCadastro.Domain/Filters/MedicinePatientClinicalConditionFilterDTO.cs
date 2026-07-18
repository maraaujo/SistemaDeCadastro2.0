using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class MedicinePatientClinicalConditionFilterDTO
    {
        public long Id { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public long? MedicineId { get; set; }
        public string MedicineName { get; set; }
        public long? PatientClinicalConditionId { get; set; }
        public string ClinicalConditionName { get; set; }
        public long? ResponsibleEmployeeId { get; set; }
        public string ResponsibleEmployeeName { get; set; }
        public TimeSpan? AdministrationTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
    }
}