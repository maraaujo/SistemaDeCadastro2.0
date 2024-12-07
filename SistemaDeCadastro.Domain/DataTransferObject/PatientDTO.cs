using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PatientDTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Document { get; set; }

        public string? Responsible { get; set; }

        public string? Phone { get; set; }

        public long IdBloodType { get; set; }
    }
}
