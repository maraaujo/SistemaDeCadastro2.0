using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class CareService
    {
        public long Id { get; set; }

        public long AppointmentId { get; set; }

        public long PatientId { get; set; }

        public long UserId { get; set; }

        public string Description { get; set; }

        public DateTime ServiceDate { get; set; }

        public string Referral { get; set; }

        public string Observations { get; set; }

        public virtual Appointment Appointment { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual LoginAccount User { get; set; }
    }
}
