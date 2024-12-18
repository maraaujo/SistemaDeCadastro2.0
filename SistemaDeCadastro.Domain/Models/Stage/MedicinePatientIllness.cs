﻿using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public partial class MedicinePatientIllness
{
    public long Id { get; set; }

    public long IdMedicine { get; set; }

    public string Dosage { get; set; } = null!;

    public int Time { get; set; }
    public long IdPatient { get; set; }

    public long IdIllness { get; set; }

    public virtual Illness IdIllnessNavigation { get; set; } = null!;

    public virtual Patient IdPatientNavigation { get; set; } = null!;

    public virtual Medicine IdMedicineNavigation { get; set; } = null!;

    public virtual ICollection<MedicinePatientIllnessHistoric> MedicinePatientIllnessHistorics { get; set; } = new List<MedicinePatientIllnessHistoric>();
}
