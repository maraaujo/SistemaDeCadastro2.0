using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateCareServiceDTO
    {
        public long AppointmentId { get; set; }

        public long UserId { get; set; }

        public string Description { get; set; }

        public DateTime ServiceDate { get; set; }

        public string Referral { get; set; }

        public string Observations { get; set; }
    }
}
