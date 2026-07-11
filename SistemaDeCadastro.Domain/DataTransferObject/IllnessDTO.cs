using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class IllnessDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Cid { get; set; }
    }
}
