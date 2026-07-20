using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
namespace SistemaDeCadastro.APP.APP
{
    public class PatientIllnessApp : IPatientIllnessApp
    {
        private readonly IPatientIllnessRepository _repo;

        public PatientIllnessApp(IPatientIllnessRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PatientIllness>> GetAll() => await _repo.GetAll();

        public async Task<PatientIllness?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreatePatientIllnessDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var newPatientIllness = new PatientIllness
                { 
                    PatientId = entity.PatientId,
                    IllnessId = entity.IllnessId,
                    Observations = entity.Observations,
                    DiagnosisDate = entity.DiagnosisDate

                };
                await _repo.Create(newPatientIllness);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdatePatientIllnessDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingEntity = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
               existingEntity.PatientId = entity.PatientId;
                existingEntity.IllnessId = entity.IllnessId;
                existingEntity.Observations = entity.Observations;
                existingEntity.DiagnosisDate = entity.DiagnosisDate;
                await _repo.Update(existingEntity);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
