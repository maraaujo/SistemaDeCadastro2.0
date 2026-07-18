using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdatePatientEmployeeDTO
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public long EmployeeId { get; set; }
        public string ResponsibilityFunction { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
