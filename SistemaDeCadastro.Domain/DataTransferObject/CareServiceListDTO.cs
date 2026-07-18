using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CareServiceListDTO
    {
        public long Id { get; set; }
        public long? AppointmentId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public long UserId { get; set; }
        public string Description { get; set; }
        public DateTime ServiceDate { get; set; }
        public string Referral { get; set; }
        public string Observations { get; set; }
    }
}