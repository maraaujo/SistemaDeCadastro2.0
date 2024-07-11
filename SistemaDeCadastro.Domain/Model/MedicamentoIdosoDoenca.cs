using Microsoft.AspNetCore.Identity;

namespace SistemaDeCadastro.Domain.Model
{
    public class MedicamentoIdosoDoenca 
    {
        
        public int Id { get; set; }
        public string MedicamentoDosagem { get; set; }
        public int MedicamentoId { get; set; }
        public Medicamento Medicamentos { get; set; }
        public int IdosoId { get; set; }
        public Idoso Idoso { get; set; }
        public int DoencaId { get; set; }
        public Doenca Doencas { get; set; }

    }
}
