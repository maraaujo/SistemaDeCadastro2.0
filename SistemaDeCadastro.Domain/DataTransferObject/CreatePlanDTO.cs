namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreatePlanDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int MaxPatients { get; set; }
        public int MaxUsers { get; set; }
        public bool Active { get; set; }
    }
}