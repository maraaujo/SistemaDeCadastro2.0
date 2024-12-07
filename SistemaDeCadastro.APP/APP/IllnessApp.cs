using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Infra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class IllnessApp : IIllnessApp
    {
        private readonly IIllnessRepository _illnessRepository;
        public IllnessApp(IIllnessRepository illnessRepository)
        {
            this._illnessRepository = illnessRepository;
        }
        public async Task<List<Illness>> GetIllnessById(long id) =>
             await this._illnessRepository.GetIllnessById(id);

        public async Task GetAnyIllnessIfTheValor(string illness) =>
            await this._illnessRepository.GetAnyIllnessIfTheValor(illness);

        public async Task<ApiResponse> CreateIllness(IllnessDTO illness)
        {
            ApiResponse ret = new(); try
            {
                Illness newIllness = new(); 
                newIllness.Id = illness.Id;
                newIllness.Name = illness.Name;
                await this._illnessRepository.CreateIllness(newIllness);    
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }

        public async Task<ApiResponse> UpdateIllness(IllnessDTO illness)
        {
            ApiResponse ret = new(); try
            {
                Illness newIllness = (await _illnessRepository.GetIllnessById(illness.Id)).FirstOrDefault(); 
                newIllness.Name = illness.Name;
                await this._illnessRepository.UpdateIllness(newIllness);
            }
            catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }

        public async Task<ApiResponse> DeleteIllness(IllnessDTO illness)
        {
            ApiResponse ret = new();
            try
            {
                Illness deleteIllness = (await _illnessRepository.GetIllnessById(illness.Id)).FirstOrDefault();
                await this._illnessRepository.DeleteIlless(deleteIllness);
            }

               catch (Exception err)
            {
                ret.ErrorMessage = err.Message;
                ret.Success = false;
            }

            return ret;
        }

        public async Task<ApiResponse> GetAllIllness()
        {
            ApiResponse ret = new();
            try
            {
                ret.Data = await _illnessRepository.GetAllIllness();
                ret.Success = true;
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
