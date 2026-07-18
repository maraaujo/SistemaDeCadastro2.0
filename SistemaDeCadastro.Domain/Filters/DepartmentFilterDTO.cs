namespace SistemaDeCadastro.Domain.Filters
{
    public class DepartmentFilterDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Page { get; set; } = 1;
    }
}