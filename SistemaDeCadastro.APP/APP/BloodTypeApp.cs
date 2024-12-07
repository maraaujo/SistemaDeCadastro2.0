using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.APP.APP
{
    public class BloodTypeApp : IBloodTypeApp
    {
        private readonly IBloodTypeRepository _bloodTypeRepository;
        public BloodTypeApp(IBloodTypeRepository bloodTypeRepository) 
        {
          this._bloodTypeRepository = bloodTypeRepository;
        }
        public async Task<List<BloodType>> GetBloodTypeById(long id) => 
            await this._bloodTypeRepository.GetBloodTypeById(id);

        public async Task<ApiResponse> GetAllBloodIllness ()
        {
            ApiResponse ret = new();
            try
            {
                ret.Data = await _bloodTypeRepository.FindAll();
                ret.Success = true;
            }


            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }
        public async Task<ApiResponse> CreateBloodType(BloodTypeDTO bloodType)
        {
            ApiResponse ret = new(); try
            {
                BloodType newBloodType = new();
                newBloodType.Id = bloodType.Id;
                newBloodType.Name = bloodType.Name;
                await this._bloodTypeRepository.CreateBooldType(newBloodType);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }
        public async Task<ApiResponse> DeleteIllness(BloodTypeDTO bloodType)
        {
            ApiResponse ret = new();
            try
            {
                BloodType deletebloodType = (await _bloodTypeRepository.GetBloodTypeById(bloodType.Id)).FirstOrDefault();
                await this._bloodTypeRepository.DeleteBloodtype(deletebloodType);
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
