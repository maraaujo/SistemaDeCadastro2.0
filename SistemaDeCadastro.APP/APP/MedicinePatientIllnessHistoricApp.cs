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
        public MedicinePatientIllnessHistoricApp(IMedicinePatientIllnessHistoricRepository medicinePatientIllnessHistoricRepository)
        {
            this._medicinePatientIllnessHistoricRepository = medicinePatientIllnessHistoricRepository;
        }

        public async Task<List<MedicinePatientIllnessHistoric>> GetMedicinePatientIllnessHistoricById(long id) =>
            await this._medicinePatientIllnessHistoricRepository.GetMedicinePatientIllnessHistoricById(id);

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
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }
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
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }
    }
}
