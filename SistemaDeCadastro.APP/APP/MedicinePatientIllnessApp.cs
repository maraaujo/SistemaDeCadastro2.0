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
    public class MedicinePatientIllnessApp : IMedicinePatientIllnessApp
    {
        private readonly IMedicinePatientIllnessRepository _medicinePatientIllnessRepository;
        public MedicinePatientIllnessApp(IMedicinePatientIllnessRepository _medicinePatientIllnessRepository)
        {
            this._medicinePatientIllnessRepository = _medicinePatientIllnessRepository;
        }
        public async Task<List<MedicinePatientIllness>> GetMedicinePatientIllnessesById(long id) =>
        await this._medicinePatientIllnessRepository.GetMedicinePatientIllnessesById(id);
        
        public async Task<ApiResponse> UpdateMedicinePatientIllness(MedicinePatientIllnessDTO medicinePatientIllness) 
        {
            ApiResponse ret = new();

            try
            {
                MedicinePatientIllness updateMedicinePatientIllness = 
                    (await this._medicinePatientIllnessRepository.GetMedicinePatientIllnessesById(medicinePatientIllness.Id)).FirstOrDefault();
                updateMedicinePatientIllness.IdMedicine = medicinePatientIllness.IdMedicine;
                updateMedicinePatientIllness.Dosage = medicinePatientIllness.Dosage;
                updateMedicinePatientIllness.Time = medicinePatientIllness.Time;
                await this._medicinePatientIllnessRepository.UpdateMedicinePatientIllness(updateMedicinePatientIllness);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }

        public async Task<ApiResponse> CreateMedicinePatientIllness(MedicinePatientIllnessDTO medicinePatientIllness)
        {
            ApiResponse ret = new();
            try
            {
                MedicinePatientIllness newMedicinePatientIllness = new();
                newMedicinePatientIllness.Id = medicinePatientIllness.Id;
                newMedicinePatientIllness.IdMedicine = medicinePatientIllness.IdMedicine;
                newMedicinePatientIllness.Dosage = medicinePatientIllness.Dosage;
                newMedicinePatientIllness.Time = medicinePatientIllness.Time;
                await this._medicinePatientIllnessRepository.CreateMedicinePatientIllness(newMedicinePatientIllness);

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
