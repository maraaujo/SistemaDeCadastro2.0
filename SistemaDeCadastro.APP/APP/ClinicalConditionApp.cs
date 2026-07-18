using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class ClinicalConditionApp : IClinicalConditionApp
    {
        private readonly IClinicalConditionRepository _clinicalConditionRepository;

        public ClinicalConditionApp(IClinicalConditionRepository clinicalConditionRepository)
        {
            _clinicalConditionRepository = clinicalConditionRepository;
        }

        public async Task<List<ClinicalCondition>> GetAll() => await _clinicalConditionRepository.GetAll();

        public async Task<ClinicalCondition?> GetById(long id) => (await _clinicalConditionRepository.FindBy(c => c.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateClinicalConditionDTO clinicalCondition)
        {
            try
            {
                var entity = new ClinicalCondition
                {
                    Name = clinicalCondition.Name,
                    Description = clinicalCondition.Description,
                    Type = clinicalCondition.Type
                };
                await _clinicalConditionRepository.Create(entity);
                return new ApiResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse> Update(UpdateClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingEntity = (await _clinicalConditionRepository.FindBy(c => c.Id == entity.Id)).FirstOrDefault();
                if (existingEntity != null)
                {
                    existingEntity.Name = entity.Name;
                    existingEntity.Description = entity.Description;
                    existingEntity.Type = entity.Type;
                    await _clinicalConditionRepository.Update(existingEntity);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Clinical condition not found.";
                }

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;  
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _clinicalConditionRepository.FindBy(c => c.Id == id)).FirstOrDefault(); if (entity != null) await _clinicalConditionRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
