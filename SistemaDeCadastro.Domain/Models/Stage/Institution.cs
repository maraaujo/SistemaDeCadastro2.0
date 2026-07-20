using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class Institution
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
