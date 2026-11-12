using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class DetailsPatientDTO
    {
        public PatientDTO Patient { get; set; }
        public List<ResponsibleListDTO> Responsibles { get; set; }
        public List<ClinicalConditionDTO> ClinicalConditions { get; set; }
        public List<MedicineDTO> Medicines { get; set; }
        // Prescricoes do acolhido com o id de cada uma (usado na tela de edicao).
        public List<PatientScheduledMedicineDTO> ScheduledMedicines { get; set; } = new();
        public List<AppointmentListDTO> Appointments { get; set; }
        public BloodTypeDTO BloodType { get; set; }
        public List<IllnessDTO> Illnesses { get; set; }
        public  List<CareServiceListDTO> CareService { get; set; }
    }
}
