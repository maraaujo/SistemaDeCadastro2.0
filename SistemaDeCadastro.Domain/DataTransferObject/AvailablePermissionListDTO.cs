namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class AvailablePermissionListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public bool Active { get; set; }
    }
}