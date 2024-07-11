using SistemaDeCadastro.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class ElderlyMedicineDTO
    {
        public int Id { get; set; }
        public string MedicamentoDosagem { get; set; }
        public int MedicamentoId { get; set; }
        public string Medicamentos { get; set; }
        public int IdosoId { get; set; }
        public Idoso Idoso { get; set; }
        public string IdosoNome  { get; set; }
        public string IdosoSobrenome { get; set; }
        public int DoencaId { get; set; }
        public Doenca Doencas { get; set; }
    }
}
