using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class Vias
    {
        public int Cod { get; set; }
        public string Nome { get; set; }

        // Relationship: one Vias can have multiple Posologias
        public virtual ICollection<Posologia> Posologias { get; set; }
    }
}
