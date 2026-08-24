using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PatientDTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Phone { get; set; }

        public string? Document { get; set; }

        public string? Gender { get; set; }

        public string? Cpf { get; set; }

        public string? Observations { get; set; }

        public long? BloodTypeId { get; set; }

        public List<UpdateResponsibleDTO>? Responsibles { get; set; } = new();

        public List<UpdatePatientClinicalConditionDTO>? ClinicalConditions { get; set; } = new();

        public List<UpdatePatientMedicineDTO>? ScheduledMedicines { get; set; } = new();
    }
}
