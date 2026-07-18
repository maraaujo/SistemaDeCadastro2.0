namespace SistemaDeCadastro.Domain.Filters
{
    public class AvailablePermissionFilterDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public bool? Active { get; set; }
        public int Page { get; set; } = 1;
    }
}