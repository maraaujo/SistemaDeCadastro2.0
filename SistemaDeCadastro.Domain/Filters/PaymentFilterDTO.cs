using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class PaymentFilterDTO
    {
        public long Id { get; set; }
        public long? AppointmentId { get; set; }
        public long? UserId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; }
        public string ExternalTransactionId { get; set; }
        public int Page { get; set; } = 1;
    }
}