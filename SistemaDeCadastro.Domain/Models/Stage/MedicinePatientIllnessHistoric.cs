using System;

namespace SistemaDeCadastro.Domain.Models.Stage;

public class MedicinePatientIllnessHistoric
{
    public long Id { get; set; }

    public long IdMedicinePatientIllness { get; set; }
    //horario em que realmente foi administrado o remédio 
    public DateTime? LastTime { get; set; }
   

    // Navigation
    public MedicinePatientIllness? MedicinePatientIllness { get; set; }
}
