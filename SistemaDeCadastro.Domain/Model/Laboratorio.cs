using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class Laboratorio
    {
        public int Cod { get; set; }
        public string  Nome  { get; set; }
       
        public virtual ICollection<Medicamento> Medicamentos { get; set; }
    }
}
