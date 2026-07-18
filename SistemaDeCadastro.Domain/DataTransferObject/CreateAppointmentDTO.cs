using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateAppointmentDTO
    {
        public long UserId { get; set; }


        public long PatientId { get; set; }

        public string AppointmentType { get; set; }

        public DateTime DateTime { get; set; }

        public string Responsible { get; set; }

        public string Status { get; set; }

        public string Observations { get; set; }
    }
}
