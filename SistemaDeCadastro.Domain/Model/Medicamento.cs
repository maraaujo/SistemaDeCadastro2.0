
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SistemaDeCadastro.Domain.Model
{
    public class Medicamento 
    {
        
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Laboratorio { get; set; }
        public string UnidadeMedida { get; set; }
        public decimal Valor { get; set; }
        public ICollection<Ministracao> Ministracoes { get; set; }
        public ICollection<MedicamentoIdosoDoenca> MedicamentoIdosoDoencas { get; set; }
    }
}
