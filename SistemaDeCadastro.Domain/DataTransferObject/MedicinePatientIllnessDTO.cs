using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Domain.DataTransferObject
{
    public class MedicinePatientIllnessDTO
    {
        public long Id { get; set; }

        public long idPatient { get; set; }
        public string Patient { get; set; } = null!;
        public long IdIllness { get; set; }
        public string Illness { get; set; } = null!;

        public long IdMedicine { get; set; }
        public string Medicine { get; set; } = null!;

        public string Dosage { get; set; } = null!;

        public int Time { get; set; }
        public DateTime? NextMedicineTime
        {
            get => MedicineHistoric.Any() ? MedicineHistoric.OrderByDescending(c => c.LastTime).First().LastTime
                .AddHours(Time) : null;
        }
        //preencher o Time e o NextdicineTime

        //O que foi feito abaixo chama-se inicializacao de variavel.
        //Quando vce instancia a MedicinePatientIlnessDTO, se voce nao fizer a inicializacao abaixo     
        //a propriedade MedicineHistoric abaixo iria ficar como null NULL. Isso eh ruim posteriomente quando 
        //for efetuar logicas em cima dessa lista (por ex: if lista == null)

        public List<MedicinePatientHistoricDTO> MedicineHistoric { get; set; } = new List<MedicinePatientHistoricDTO>(); //intanciando a lista para que ela não fiquei como nula.
    }
}
