using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicinePatientIllnessHistoricApp : IMedicinePatientIllnessHistoricApp
    {
        private readonly IMedicinePatientIllnessHistoricRepository _medicinePatientIllnessHistoricRepository;
        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de historico de medicamento por doenca do acolhido.
        /// </summary>
        public MedicinePatientIllnessHistoricApp(IMedicinePatientIllnessHistoricRepository medicinePatientIllnessHistoricRepository)
        {
            this._medicinePatientIllnessHistoricRepository = medicinePatientIllnessHistoricRepository;
        }

        /// <summary>
        /// Devolve uma lista com o registro de historico do Id informado (vazia se nao existir).
        /// </summary>
        public async Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id) =>
            await this._medicinePatientIllnessHistoricRepository.GetMedicinePatientIllnessHistoricById(id);

        /// <summary>
        /// Cria um registro de historico com o Id e o IdMedicinePatientIllness vindos do DTO.
        /// Observacao: mesmo quando grava, o retorno nao marca Success = true.
        /// </summary>
        public async Task<ApiResponse> CreateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoricDTO medicinePatientIllnessHistoric)
        {
            ApiResponse ret = new();
            try
            {
                MedicinePatientIllnessHistoric newMedicinePatientIllnessHistoric = new();
                newMedicinePatientIllnessHistoric.Id = medicinePatientIllnessHistoric.Id;
                newMedicinePatientIllnessHistoric.IdMedicinePatientIllness = medicinePatientIllnessHistoric.IdMedicinePatientIllness;
                await this._medicinePatientIllnessHistoricRepository.CreateMedicinePatientIllnessHistoric(newMedicinePatientIllnessHistoric);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }
            return ret;
        }
        /// <summary>
        /// Atualiza o IdMedicinePatientIllness de um registro de historico existente.
        /// Observacao: nao marca Success = true ao gravar e, se o Id nao existir, cai no catch (NullReferenceException).
        /// </summary>
        public async Task<ApiResponse> UpdateMedicinePatientIllnessHistoric(MedicinePatientIllnessHistoricDTO medicinePatientIllnessHistoric)
        {
            ApiResponse ret = new();
            try
            {
                MedicinePatientIllnessHistoric updateMedicinePatientIllnessHistoric = (await this._medicinePatientIllnessHistoricRepository.GetMedicinePatientIllnessHistoricById(medicinePatientIllnessHistoric.Id)).FirstOrDefault();

                updateMedicinePatientIllnessHistoric.IdMedicinePatientIllness = medicinePatientIllnessHistoric.IdMedicinePatientIllness;
                await this._medicinePatientIllnessHistoricRepository.UpdateMedicinePatientIllnessHistoric(updateMedicinePatientIllnessHistoric);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = ApiErrors.From(err);
                ret.Success = false;
            }
            return ret;
        }
    }
}
