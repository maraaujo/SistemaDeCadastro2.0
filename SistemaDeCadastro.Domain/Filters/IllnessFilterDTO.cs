namespace SistemaDeCadastro.Domain.Filters
{
    public class IllnessFilterDTO
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Cid { get; set; }
        public int Page { get; set; } = 1;
    }
}