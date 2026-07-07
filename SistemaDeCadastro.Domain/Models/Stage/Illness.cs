using System;
using System.Collections.Generic;

namespace SistemaDeCadastro.Domain.Models.Stage;

public partial class Illness
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Cid { get; set; }

    public virtual ICollection<PatientIllness> PatientIllnesses { get; set; }
}
