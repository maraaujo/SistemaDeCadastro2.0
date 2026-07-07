using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicinePatientIllnessHistoricRepository : BaseRepository<MedicinePatientIllnessHistoric>, IMedicinePatientIllnessHistoricRepository
    {
        public MedicinePatientIllnessHistoricRepository(SistemaDeCadastroContext context)
            : base(context)
        {
        }

        public async Task CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric)
        {
            await this.Create(medicinePatientIllnessHistoric);
        }

        public async Task UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric)
        {
            await this.Update(medicinePatientIllnessHistoric);
        }

        public async Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }
    }
}
