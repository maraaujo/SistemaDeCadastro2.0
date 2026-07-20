using System;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class SubscriptionPayment
    {
        public long Id { get; set; }
        public long SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ExternalPaymentId { get; set; }
        public string Observations { get; set; }

        public virtual Subscription Subscription { get; set; }
    }
}
