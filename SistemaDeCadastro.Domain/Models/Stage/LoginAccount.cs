using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class LoginAccount
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

        public virtual ICollection<Appointment> Appointments { get; set; }

        public virtual ICollection<CareService> CareServices { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
