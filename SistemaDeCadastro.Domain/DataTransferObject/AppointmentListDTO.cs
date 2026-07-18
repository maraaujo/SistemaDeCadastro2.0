using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class AppointmentListDTO
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; }
        public string AppointmentType { get; set; }
        public DateTime DateTime { get; set; }
        public string Responsible { get; set; }
        public string Status { get; set; }
        public string Observations { get; set; }
    }
}