using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicinePatientIllnessHistoricRepository : BaseRepository<MedicinePatientIllnessHistoric>, IMedicinePatientIllnessHistoricRepository
    {
        public MedicinePatientIllnessHistoricRepository(SistemaCadastroContext context) : base(context)
        {
        }

        public async Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }

        public async Task CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric)
        {
            await this.Create(medicinePatientIllnessHistoric);
        }
        public async Task UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric)
        {
            await this.Update(medicinePatientIllnessHistoric);
        }
        public async Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoric(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }
    }
}
