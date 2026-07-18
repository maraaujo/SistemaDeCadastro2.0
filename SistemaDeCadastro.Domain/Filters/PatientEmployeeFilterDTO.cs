using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class PatientEmployeeFilterDTO
    {
        public long Id { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ResponsibilityFunction { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
    }
}