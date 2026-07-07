using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class Payment
    {
        public long Id { get; set; }

        public long AppointmentId { get; set; }

        public long UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Status { get; set; }

        public string ExternalTransactionId { get; set; }

        public virtual Appointment Appointment { get; set; }

        public virtual LoginAccount User { get; set; }
    }
}
