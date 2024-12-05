using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public partial class PatientIllness
{
    public long Id { get; set; }

    public long IdPatient { get; set; }

    public long IdIlleness { get; set; }

    public virtual Illness IdIllenessNavigation { get; set; } = null!;

    public virtual Patient IdPatientNavigation { get; set; } = null!;

    public virtual ICollection<MedicinePatientIllness> MedicinePatientIllnesses { get; set; } = new List<MedicinePatientIllness>();
}
