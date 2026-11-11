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
        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public MedicinePatientIllnessHistoricRepository(SistemaDeCadastroContext context)
            : base(context)
        {
        }

        /// <summary>Grava um novo registro de historico (atalho para Create do BaseRepository).</summary>
        public async Task CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric)
        {
            await this.Create(medicinePatientIllnessHistoric);
        }

        /// <summary>Atualiza um registro de historico (atalho para Update do BaseRepository).</summary>
        public async Task UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoric medicinePatientIllnessHistoric)
        {
            await this.Update(medicinePatientIllnessHistoric);
        }

        /// <summary>Busca o registro de historico pelo id; devolve uma lista com 0 ou 1 item.</summary>
        public async Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }
    }
}
