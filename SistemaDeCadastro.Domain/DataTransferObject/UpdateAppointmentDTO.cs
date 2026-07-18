using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdateAppointmentDTO
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public long UserId { get; set; }
        public string AppointmentType { get; set; }
        public DateTime DateTime { get; set; }
        public string Responsible { get; set; }
        public string Status { get; set; }
        public string Observations { get; set; }
    }
}
