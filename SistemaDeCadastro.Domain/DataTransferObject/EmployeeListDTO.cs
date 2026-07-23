using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class EmployeeListDTO
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Cpf { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string?  Email { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}