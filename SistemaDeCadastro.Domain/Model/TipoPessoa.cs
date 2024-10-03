using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class TipoPessoa
    {
        public int Cod { get; set; }
        public string Nome { get; set; }


        public virtual ICollection<PessoaTipoPessoa> PessoaTipoPessoas { get; set; }
    }
}
