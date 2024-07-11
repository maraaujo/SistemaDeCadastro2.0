using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Model
{
    public class Ministracao
    {
        public string Id { get; set; }
        public string Posologia { get; set; }
        public int MedicamentoId { get; set; }  // Corrected the typo
        public int IdosoId { get; set; }
        public DateTime Data { get; set; }
        public Idoso Idoso { get; set; }  // Each Ministracao is related to one Idoso
        public Medicamento Medicamento { get; set; }  // Each Ministracao is related to one Medicamento
    }
}
