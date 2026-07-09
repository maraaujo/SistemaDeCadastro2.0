using SistemaDeCadastro.APP.Interface;
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

        public async Task<ApiResponse> Create(ClinicalCondition entity)
        {
            var ret = new ApiResponse();
            try { await _clinicalConditionRepository.Create(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Update(ClinicalCondition entity)
        {
            var ret = new ApiResponse();
            try { await _clinicalConditionRepository.Update(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _clinicalConditionRepository.FindBy(c => c.Id == id)).FirstOrDefault(); if (entity != null) await _clinicalConditionRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
