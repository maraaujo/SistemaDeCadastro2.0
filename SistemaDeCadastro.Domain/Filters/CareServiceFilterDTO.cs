using System;
using System.Globalization;

namespace SistemaDeCadastro.Domain.Filters
{
    public class CareServiceFilterDTO
    {
        public long? Id { get; set; }
        public long? AppointmentId { get; set; }

        public long? PatientId { get; set; }
        public long? UserId { get; set; }
        public string? AppointmentType { get; set; }
        public string? Description { get; set; }
        public string? PatientName { get; set; }
        public DateTime? ServiceDate { get; set; }

        public string? Referral { get; set; }

        public string? Observations { get; set; }
        public int Page { get; set; } = 1;
    }
}