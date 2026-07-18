using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UserPermissionListDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserEmail { get; set; }
        public long PermissionId { get; set; }
        public string PermissionName { get; set; }
        public DateTime? GrantedAt { get; set; }
    }
}