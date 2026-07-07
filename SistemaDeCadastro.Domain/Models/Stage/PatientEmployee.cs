using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class PatientEmployee
    {
        public long Id { get; set; }

        public long PatientId { get; set; }

        public long EmployeeId { get; set; }

        public string ResponsibilityFunction { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
