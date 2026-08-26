using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    // Projeção segura de LoginAccount para respostas de API.
    // Nunca inclui PasswordHash — só a entidade completa (Models.Stage.LoginAccount)
    // deve carregar o hash, e só para uso interno (autenticação).
    public class LoginAccountDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public long? InstitutionId { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }
    }
}
