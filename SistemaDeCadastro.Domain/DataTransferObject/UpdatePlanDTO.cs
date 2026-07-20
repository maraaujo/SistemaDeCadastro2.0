namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class UpdatePlanDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int MaxPatients { get; set; }
        public int MaxUsers { get; set; }
        public bool Active { get; set; }
    }
}