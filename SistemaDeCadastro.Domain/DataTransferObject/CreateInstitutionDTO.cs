namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateInstitutionDTO
    {
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
    }
}