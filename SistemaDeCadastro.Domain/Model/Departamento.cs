using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class Departamento 
    {
        
        public int Id { get; set; }
        public string Nome { get; set; }
        public ICollection<Funcionario> Funcionarios { get; set; }
    }
}