using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public partial class Medicine
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<MedicinePatientIllness> MedicinePatientIllnesses { get; set; } = new List<MedicinePatientIllness>();
}
