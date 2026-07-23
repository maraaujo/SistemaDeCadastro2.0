using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class AppointmentFilterDTO
    {
        public long Id { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public long? UserId { get; set; }
        public string AppointmentType { get; set; }
        public DateTime? DateTime { get; set; }
        public string Responsible { get; set; }
        public string Observations { get; set; }
        public string Status { get; set; }
        public int Page { get; set; } = 1;
    }
}