namespace SistemaDeCadastro.Domain.Filters
{
    public class ClinicalConditionFilterDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Page { get; set; } = 1;
    }
}