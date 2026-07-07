using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class PatientIllness
    {
        public long Id { get; set; }

        public long PatientId { get; set; }

        public long IllnessId { get; set; }

        public DateTime? DiagnosisDate { get; set; }

        public string Observations { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Illness Illness { get; set; }
    }
}
