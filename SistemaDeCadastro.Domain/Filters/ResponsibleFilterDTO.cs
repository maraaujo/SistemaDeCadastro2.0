namespace SistemaDeCadastro.Domain.Filters
{
    public class ResponsibleFilterDTO
    {
        public long Id { get; set; }
        public long? PatientId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Relationship { get; set; }
        public string Address { get; set; }
        public int Page { get; set; } = 1;
    }
}