using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateSubscriptionPaymentDTO
    {
        public long SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ExternalPaymentId { get; set; }
        public string Observations { get; set; }
    }
}