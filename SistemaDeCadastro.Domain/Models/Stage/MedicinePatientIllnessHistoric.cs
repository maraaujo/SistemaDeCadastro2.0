using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public partial class MedicinePatientIllnessHistoric
{
    public long Id { get; set; }

    public long IdMedicinePatientIllness { get; set; }

    public DateTime LastTime { get; set; }

    public virtual MedicinePatientIllness IdMedicinePatientIllnessNavigation { get; set; } = null!;
}
