using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicamentoPosologiaDTO
    {
        public int MedicamentoCod { get; set; }
        public string MedicamentoNome { get; set; }
        public DateTime PosologiaDataInicial { get; set; }
        public DateTime? PosologiaDataFinal { get; set; }
        public string PosologiaDose { get; set; }
        public string ViasAplicar { get; set; }
    }
}
