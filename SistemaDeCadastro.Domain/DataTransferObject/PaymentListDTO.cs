using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PaymentListDTO
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; }
        public string ExternalTransactionId { get; set; }
    }
}