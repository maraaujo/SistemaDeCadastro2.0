using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MorbidadeDTO
    {
        public int Cod { get; set; }
        public string Nome { get; set; }
        public int PessoaCod { get; set; }
        
    }
}
