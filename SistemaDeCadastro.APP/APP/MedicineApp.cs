using Microsoft.Identity.Client;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.APP
{
    public class MedicineApp : IMedicineApp
    {
        private readonly IMedicineRepository _medicineRepository;
        public MedicineApp(IMedicineRepository medicineRepository)
        {
            this._medicineRepository = medicineRepository;  
        }

        public async Task<List<Medicine>> GetAll() =>
            await this._medicineRepository.GetAll();
        public async Task<List<Medicine>> GetMedicineId(long id) =>
            await this._medicineRepository.GetMedicineById(id);
        public async Task GetMedicineByAnyValorString(string medicine) =>
            await this._medicineRepository.GetMedicineByAnyValorString(medicine);

        public async Task<ApiResponse> CreateMedicine(CreateMedicineDTO medicine)
        {
            ApiResponse ret = new();
            try
            {
                Medicine newMedicine = new();
                newMedicine.Frequency = medicine.Frequency;
                newMedicine.Description = medicine.Description;
                newMedicine.Dosage = medicine.Dosage;
                newMedicine.Name = medicine.Name;
                newMedicine.AdministrationRoute = medicine.AdministrationRoute;
                await this._medicineRepository.CreateMedicine(newMedicine);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }
            return ret;
        }

        public async Task<ApiResponse> UpdateMedicine(UpdateMedicineDTO medicine)
        {
            ApiResponse ret = new();

            try
            {
                Medicine updateMedicine = (await this._medicineRepository.GetMedicineById(medicine.Id)).FirstOrDefault();
             
                updateMedicine.Name = medicine.Name;
                updateMedicine.Description = medicine.Description;
                updateMedicine.Dosage = medicine.Dosage;
                updateMedicine.Frequency = medicine.Frequency;
                updateMedicine.AdministrationRoute = medicine.AdministrationRoute;
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
        public async Task<PagedMedicineDTO> GetMedicineByFilter(MedicineFilterDTO filter)
        { return await this._medicineRepository.GetMedicineByFilter(filter); }
        
    }
}
