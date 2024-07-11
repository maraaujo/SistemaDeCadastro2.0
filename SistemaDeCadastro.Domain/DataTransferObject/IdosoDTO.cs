using SistemaDeCadastro.Domain.Model;


namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class IdosoDTO
    {
        public IdosoDTO()
        {
            
        }
        public IdosoDTO(Idoso idoso)
        {
            
            this.Nome = idoso.Nome;
            this.Sobrenome = idoso.Sobrenome;
            this.Cpf = idoso.Cpf;
           
         
        }


        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public DateTime DataDeNascimento { get; set; }
        public string Cpf { get; set; }
        public Doenca Doenca { get; set; }
        public Familia Familia { get; set; }
        public Medicamento Medicamento { get; set; }
    }
}
