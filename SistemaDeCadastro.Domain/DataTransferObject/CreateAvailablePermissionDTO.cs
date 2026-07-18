using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateAvailablePermissionDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Module { get; set; }

        public bool Active { get; set; } = true;
    }
}
