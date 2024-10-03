using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class PessoaTipoPessoa
    {
        public int Cod {  get; set; }
        public int CodPessoa { get; set; }
        public int CodTipoPessoa { get; set; }

        public virtual Pessoa Pessoa { get; set; }
        public virtual TipoPessoa TipoPessoa { get; set; }
    }
}
