namespace SistemaDeCadastro.Domain.Filters
{
    public class MedicineFilterDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public int? Frequency { get; set; }
        public string Description { get; set; }
        public string AdministrationRoute { get; set; }
        public int Page { get; set; } = 1;
    }
}