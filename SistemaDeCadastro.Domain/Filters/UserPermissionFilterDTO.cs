using System;

namespace SistemaDeCadastro.Domain.Filters
{
    public class UserPermissionFilterDTO
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string UserEmail { get; set; }
        public long? PermissionId { get; set; }
        public string PermissionName { get; set; }
        public DateTime? GrantedAt { get; set; }
        public int Page { get; set; } = 1;
    }
}