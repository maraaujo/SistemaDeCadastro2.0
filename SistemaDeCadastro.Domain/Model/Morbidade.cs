using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class Morbidade
    {
        public int Cod { get; set; }
        public string Nome { get; set; }
        public int PessoaCod { get; set; }

        public virtual Pessoa Pessoa { get; set; }
        public virtual ICollection<MedicamentoMorbidade> MedicamentoMorbidades { get; set; }
    }
}
