using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class AskInternalAssistantDTO
    {
        public long PatientId { get; set; }

        public string Question { get; set; }

        public DateTime? ReferenceDate { get; set; }
    }
}
