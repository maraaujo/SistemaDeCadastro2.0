using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateAccessLogDTO
    {
        public long UserId { get; set; }

        public string Action { get; set; }

        public DateTime DateTime { get; set; }

        public string IpAddress { get; set; }

        public string Observation { get; set; }
    }
}
