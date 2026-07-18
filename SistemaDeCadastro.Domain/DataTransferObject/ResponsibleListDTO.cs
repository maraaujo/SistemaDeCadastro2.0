namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class ResponsibleListDTO
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Relationship { get; set; }
        public string Address { get; set; }
    }
}