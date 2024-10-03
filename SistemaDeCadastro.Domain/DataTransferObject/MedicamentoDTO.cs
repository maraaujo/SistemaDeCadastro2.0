using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicamentoDTO
    {
        public int Cod { get; set; }
        public string Nome { get; set; }
        public int LaboratorioCod { get; set; }
        public  int MorbidadeCod { get; set; }
        public MorbidadeDTO Morbidade { get; set; }
       
    }
}
