﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.Model
{
    public class Medicamento
    {
        public int Cod { get; set; }
        public string Nome { get; set; }

        public int LaboratorioCod  { get; set; }
        public virtual Laboratorio Laboratorio { get; set; }

        public virtual ICollection<MedicamentoMorbidade> MedicamentoMorbidades { get; set; }
        public virtual ICollection<Posologia> Posologias { get; set; }
        public virtual Laboratorio Laboratorios { get; set; }
    }
}
