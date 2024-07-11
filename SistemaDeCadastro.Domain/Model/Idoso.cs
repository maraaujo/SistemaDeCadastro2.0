using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class Idoso 
    {
       

        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public DateTime DataDeNascimento { get; set; }
        public string Cpf { get; set; }
       
        public ICollection<Familia> Familias { get; set; }
        public ICollection<MedicamentoIdosoDoenca> MedicamentoIdosoDoencas { get; set; }
        public ICollection<IdosoDoenca> IdosoDoencas { get; set; }
        public ICollection<IdosoFuncionario> Funcionarios { get; set; }

    }
}
