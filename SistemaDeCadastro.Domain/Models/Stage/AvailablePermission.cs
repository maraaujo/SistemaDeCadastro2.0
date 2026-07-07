using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class AvailablePermission
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Module { get; set; }

        public bool Active { get; set; } = true;

        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
