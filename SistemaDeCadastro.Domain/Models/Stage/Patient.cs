using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public class Patient
{
    public long Id { get; set; }

    public string Name { get; set; }

    public DateTime BirthDate { get; set; }
    public string Phone { get; set; }
    public string Document { get; set; }
    public string Gender { get; set; }

    public string Cpf { get; set; }

    public string Observations { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? BloodTypeId { get; set; }

    public virtual BloodType BloodType { get; set; }

    public virtual ICollection<Responsible> Responsibles { get; set; } = new List<Responsible>();

    public virtual ICollection<PatientClinicalCondition> PatientClinicalConditions { get; set; } = new List<PatientClinicalCondition>();

    public virtual ICollection<PatientIllness> PatientIllnesses { get; set; } = new List<PatientIllness>();

    public virtual ICollection<PatientEmployee> PatientEmployees { get; set; } = new List<PatientEmployee>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<CareService> CareServices { get; set; } = new List<CareService>();
}