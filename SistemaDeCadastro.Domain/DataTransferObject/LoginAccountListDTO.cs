using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class LoginAccountListDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }
    }
}