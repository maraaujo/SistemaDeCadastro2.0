
using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class Doenca 
    {
        
        public int Id { get; set; }
        public string? Nome { get; set; }
        public ICollection<MedicamentoIdosoDoenca> MedicamentoIdosoDoencas { get; set; }
        public ICollection<IdosoDoenca> IdosoDoencas { get; set; }
    }
}
