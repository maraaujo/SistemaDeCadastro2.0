using Microsoft.Identity.Client;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicineApp : IMedicineApp
    {
        private readonly IMedicineRepository _medicineRepository;
        public MedicineApp(IMedicineRepository medicineRepository)
        {
            this._medicineRepository = medicineRepository;  
        }

        public async Task<List<Medicine>> GetMedicineId(long id) =>
            await this.GetMedicineId(id);
        public async Task GetMedicineByAnyValorString(string medicine) =>
            await this._medicineRepository.GetMedicineByAnyValorString(medicine);

        public async Task<ApiResponse> CreateMedicine(Medicine medicine)
        {
            ApiResponse ret = new();
            try
            {
                Medicine newMedicine = new();
                newMedicine.Id = medicine.Id;
                newMedicine.Name = medicine.Name;
                await this._medicineRepository.CreateMedicine(newMedicine);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }

        public async Task<ApiResponse> UpdateMedicine(MedicineDTO medicine)
        {
            ApiResponse ret = new();

            try
            {
                Medicine updateMedicine = (await this._medicineRepository.GetMedicineById(medicine.Id)).FirstOrDefault();
                updateMedicine.Id = medicine.Id;
                updateMedicine.Name = medicine.Name;
                await this._medicineRepository.UpdateMedicine(updateMedicine);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }

        public async Task<ApiResponse> DeleteMedicine(MedicineDTO medicine)
        {
            ApiResponse ret = new();
            try
            {
                Medicine idToDelete = (await this._medicineRepository.GetMedicineById(medicine.Id)).FirstOrDefault();
                await this._medicineRepository.DeleteMedicine(idToDelete);
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
