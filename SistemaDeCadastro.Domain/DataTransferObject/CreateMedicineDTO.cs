namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreateMedicineDTO
    {
        public string Name { get; set; }
        public string Dosage { get; set; }
        public long Frequency { get; set; }
        public string Description { get; set; }
        public string AdministrationRoute { get; set; }
    }
}
