using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PessoaDTO
    {
        public int Cod { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string TipoSanguineo { get; set; }
        public string Endereco { get; set; }
        public string DataObito { get; set; }
    }
}
