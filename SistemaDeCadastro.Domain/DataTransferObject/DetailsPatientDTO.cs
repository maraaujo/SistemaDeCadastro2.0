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
        public List<AppointmentListDTO> Appointments { get; set; }
        public BloodTypeDTO BloodType { get; set; }
        public List<IllnessDTO> Illnesses { get; set; }
        public  List<CareServiceListDTO> CareService { get; set; }
    }
}
