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
        
            public long? Id { get; set; }

            public string? Name { get; set; }

            public string? ResponsibleName { get; set; }

            public string? ClinicalCondition { get; set; }

            public string? Medicine { get; set; }

            public string? Dosage { get; set; }

            public TimeSpan? Time { get; set; }
        
        public int Page { get; set; } = 1;
    }
}
