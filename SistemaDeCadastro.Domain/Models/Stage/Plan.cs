using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class Plan
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int MaxPatients { get; set; }
        public int MaxUsers { get; set; }
        public bool Active { get; set; } = true;

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
