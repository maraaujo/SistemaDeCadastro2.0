using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public partial class BloodType
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
