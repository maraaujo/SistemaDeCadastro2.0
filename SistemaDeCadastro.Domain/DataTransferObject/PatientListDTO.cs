using System;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class PatientListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Document { get; set; }
        public string Gender { get; set; }
        public string Cpf { get; set; }
        public string Observations { get; set; }
        public long? BloodTypeId { get; set; }
        public string BloodTypeName { get; set; }
        public string ResponsibleName { get; set; }
    }
}