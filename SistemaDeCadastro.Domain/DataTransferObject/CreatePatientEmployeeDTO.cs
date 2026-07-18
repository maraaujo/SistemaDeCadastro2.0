using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class CreatePatientEmployeeDTO
    {
        public long EmployeeId { get; set; }

        public string Name { get; set; }

        public string Cpf { get; set; }

        public string Position { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime AdmissionDate { get; set; }

        public long? DepartmentId { get; set; }
    }
}
