using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public class Medicine
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Dosage { get; set; }
    public long Frequency { get; set; }

    public string Description { get; set; }

    public string AdministrationRoute { get; set; }

    public virtual ICollection<MedicinePatientClinicalCondition> PatientClinicalConditionMedicines { get; set; }
}
