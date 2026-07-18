namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdateClinicalConditionDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
