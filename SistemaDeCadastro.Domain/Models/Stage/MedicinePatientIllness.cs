using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public class MedicinePatientIllness
{
    public long Id { get; set; }

    public long IdPatient { get; set; }
    public long IdIllness { get; set; }
    public long IdMedicine { get; set; }

    public string Dosage { get; set; } = string.Empty;
    public int Time { get; set; }

    // Navigation properties
    public Patient? Patient { get; set; }
    public List<MedicinePatientIllnessHistoric> MedicineHistoric { get; set; } = new();
}
