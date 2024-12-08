using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PatientFilterDTO
    {
        public string Name { get; set; }
        public string Illness { get; set; }
        public string Medicine { get; set; }
        public string Dosage { get; set; }
        public string Responsible { get; set; }
        public int Time { get; set; }
        public bool Done { get; set; }
    }
}
