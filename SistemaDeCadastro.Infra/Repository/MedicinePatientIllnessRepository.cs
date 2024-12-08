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
    public class MedicinePatientIllnessRepository : BaseRepository<MedicinePatientIllness>, IMedicinePatientIllnessRepository
    {
        public MedicinePatientIllnessRepository(Domain.SistemaCadastroContext.SistemaCadastroContext context)
            : base(context)
        {
        }

        public async Task CreateMedicinePatientIllness(MedicinePatientIllness medicinePatientIllness)
        {
            await this.Create(medicinePatientIllness);
        }
        public async Task UpdateMedicinePatientIllness(MedicinePatientIllness medicinePatientIllness)
        {
            await this.Update(medicinePatientIllness);
        }
        public async Task<List<MedicinePatientIllness>> GetMedicinePatientIllnessesById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }

    }
}
