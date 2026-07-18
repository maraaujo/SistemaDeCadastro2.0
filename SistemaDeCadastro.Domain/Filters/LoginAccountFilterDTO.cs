namespace SistemaDeCadastro.Domain.Filters
{
    public class LoginAccountFilterDTO
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public bool? Active { get; set; }
        public int Page { get; set; } = 1;
    }
}