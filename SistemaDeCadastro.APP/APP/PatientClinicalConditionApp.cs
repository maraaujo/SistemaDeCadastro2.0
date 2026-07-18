using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class PatientClinicalConditionApp : IPatientClinicalConditionApp
    {
        private readonly IPatientClinicalConditionRepository _repo;
        private readonly IPatientRepository _patientRepo;
        public PatientClinicalConditionApp(IPatientClinicalConditionRepository repo, IPatientRepository patientRepo)
        {
            _repo = repo;
            _patientRepo = patientRepo;
        }

        public async Task<List<PatientClinicalCondition>> GetAll() => await _repo.GetAll();

        public async Task<PatientClinicalCondition?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreatePatientClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                if(entity.PatientId <= 0)
                {
                    ret.ErrorMessage = "Invalid patient ID";
                    ret.Success = false;
                    return ret;
                }

                var patientClinicalCondition = new PatientClinicalCondition
                {
                    PatientId = entity.PatientId,
                    ClinicalConditionId = entity.ClinicalConditionId,
                    DiagnosisDate = entity.DiagnosisDate,
                    Observations = entity.Observations
                };

                await _repo.Create(patientClinicalCondition);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.ErrorMessage = ex.Message;
                ret.Success = false;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(PatientClinicalCondition entity)
        {
            var ret = new ApiResponse();
            try { await _repo.Update(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
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
