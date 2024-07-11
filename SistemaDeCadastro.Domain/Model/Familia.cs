using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class Familia 
    {
       
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? Numero { get; set; }
        public string? Endereco { get; set; }
        public int IdosoId { get; set; }
        public Idoso Idoso {  get; set; }
    }
}
