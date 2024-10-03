using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class Pessoa
    {
        public int Cod { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string TipoSanguineo { get; set; }
        public string Endereco { get; set; }
        public string DataObito { get; set; }
        //Propriedade para o auto relacionamento 
        public int? PessoaCod { get; set; }
        //relacioanmento com a entidade "pai"
        public virtual Pessoa PessoasPai {get; set;}
        //relacionamento com as entidades "filhas"
        public virtual ICollection<Pessoa> PessoasFilhos { get; set; }
        public virtual ICollection<PessoaTipoPessoa> PessoaTipoPessoas { get; set; }
        public virtual ICollection<Morbidade> Morbidades { get; set; }
        public virtual ICollection<Posologia> Posologias { get; set; }
    }
}
