using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class Funcionario 
    {
       
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Documento { get; set; }
        public  string Email { get; set; }
        public string Senha { get; set; }
        public Departamento Departamento { get; set; }
        public int DepartamentoId { get; set; }
        public ICollection<IdosoFuncionario> Idosos { get; set; }
    }
}