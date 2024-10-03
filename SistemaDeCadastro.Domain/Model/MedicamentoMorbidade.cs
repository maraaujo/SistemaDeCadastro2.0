using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class MedicamentoMorbidade
    {
        public int MedicamentoCod { get; set; }
        public int MorbidadeCod { get; set; }

    
        public virtual Medicamento Medicamento { get; set; }
        public virtual Morbidade Morbidade { get; set; }
    }
}
