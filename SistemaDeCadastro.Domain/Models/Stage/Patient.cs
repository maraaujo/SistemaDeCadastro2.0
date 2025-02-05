using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public class Patient
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Document { get; set; }

    public string? Responsible { get; set; }

    public string? Phone { get; set; }

    public long IdBloodType { get; set; }

    public virtual BloodType IdBloodTypeNavigation { get; set; } = null!;

    public virtual ICollection<MedicinePatientIllness> MedicinePatientIllnesses { get; set; } = new List<MedicinePatientIllness>();
}
