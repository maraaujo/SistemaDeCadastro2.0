using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class Responsible
    {
        public long Id { get; set; }

        public long PatientId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Relationship { get; set; }

        public string Address { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
