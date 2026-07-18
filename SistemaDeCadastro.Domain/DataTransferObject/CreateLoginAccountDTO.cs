using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateLoginAccountDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string UserType { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool Active { get; set; } = true;


        public virtual ICollection<UserPermission> UserPermissions { get; set; }

        public virtual ICollection<AccessLog> AccessLogs { get; set; }

    }
}
