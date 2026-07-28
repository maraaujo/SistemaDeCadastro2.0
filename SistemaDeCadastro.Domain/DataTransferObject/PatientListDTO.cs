using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

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
        public string ClinicalCondition { get; set; }
        public string MedicinePatientClinicalCondition { get; set; }
        public string  Medicine { get; set; }
        public TimeSpan? Time { get; set; }
        public string Dosage { get; set; }
        public long? BloodTypeId { get; set; }
        public string BloodTypeName { get; set; }
        public string ResponsibleName { get; set; }
    }
}