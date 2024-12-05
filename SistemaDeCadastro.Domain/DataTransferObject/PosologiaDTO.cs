﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PosologiaDTO
    {
        public int Cod { get; set; }
        public DateTime DataInicial { get; set; }
        public string Dose { get; set; }
        public DateTime? DataFinal { get; set; }
        public TimeSpan? Intervalo { get; set; }
        public int PessoaCod { get; set; }
        public int MedicamentoCod { get; set; }
        public int ViasCod { get; set; }

    }
}