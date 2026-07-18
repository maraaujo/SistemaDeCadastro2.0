using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UserPermissionDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long PermissionId { get; set; }

        public DateTime GrantedAt { get; set; }
    }
}
