using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreatePatientEmployeeDTO
    {
        public long Id { get; set; }

        public long PatientId { get; set; }

        public long EmployeeId { get; set; }

        public string ResponsibilityFunction { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
