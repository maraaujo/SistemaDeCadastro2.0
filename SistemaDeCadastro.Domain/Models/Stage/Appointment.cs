using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class Appointment
    {
        public long Id { get; set; }

        public long PatientId { get; set; }

        public long UserId { get; set; }

        public string AppointmentType { get; set; }

        public DateTime DateTime { get; set; }

        public string Responsible { get; set; }

        public string Status { get; set; }

        public string Observations { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual LoginAccount User { get; set; }

        public virtual ICollection<CareService> CareServices { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
