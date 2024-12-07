using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PatientIllnessDTO
    {
      public long Id { get; set; }

      public long IdPatient { get; set; }

      public long IdIlleness { get; set; }
    }
}
