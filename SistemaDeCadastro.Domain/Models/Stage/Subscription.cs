using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class Subscription
    {
        public long Id { get; set; }
        public long InstitutionId { get; set; }
        public long PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string ExternalSubscriptionId { get; set; }

        public virtual Institution Institution { get; set; }
        public virtual Plan Plan { get; set; }
        public virtual ICollection<SubscriptionPayment> SubscriptionPayments { get; set; }
    }
}
