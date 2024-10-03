using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicamentoMorbidadeDTO
    {
        public int MedicamentoCod { get; set; }
        public int MorbidadeCod { get; set; }
        public string NomeMorbidade { get; set; }
        public string NomeMedicamento { get; set; }
        
    }
}
