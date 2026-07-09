using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeCadastro.Domain.Models.Stage
{
    public class MedicinePatientClinicalCondition
    {
        public long Id { get; set; }

        public long MedicineId { get; set; }

        public long PatientClinicalConditionId { get; set; }

        public string PrescribedDosage { get; set; }

        [Column("frequencia")]
        public string Frequency { get; set; }

        [Column("horario_administracao")]
        public TimeSpan? AdministrationTime { get; set; }

        [Column("id_funcionario_responsavel")]
        public long? ResponsibleEmployeeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Observations { get; set; }

        public virtual Medicine Medicine { get; set; }

        public virtual PatientClinicalCondition PatientClinicalCondition { get; set; }

        public virtual Employee? ResponsibleEmployee { get; set; }
    }
}