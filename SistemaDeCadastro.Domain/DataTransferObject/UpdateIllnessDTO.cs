namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdateIllnessDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cid { get; set; }
    }
}
