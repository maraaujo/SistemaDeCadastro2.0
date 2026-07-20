using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class PatientEmployeeApp : IPatientEmployeeApp
    {
        private readonly IPatientEmployeeRepository _repo;

        public PatientEmployeeApp(IPatientEmployeeRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PatientEmployee>> GetAll() => await _repo.GetAll();

        public async Task<PatientEmployee?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreatePatientEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try { 
            var newEntity = new PatientEmployee
            {
                EmployeeId = entity.EmployeeId,
                PatientId = entity.PatientId,
                ResponsibilityFunction = entity.ResponsibilityFunction,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };
            await _repo.Create(newEntity);
            ret.Success = true;
        }
        catch (Exception ex)
        {
            ret.Success = false;
            ret.ErrorMessage = ex.Message;
        }
        return ret;
    }

        public async Task<ApiResponse> Update(UpdatePatientEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try { 
            var existingEntity = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                existingEntity.EmployeeId = entity.EmployeeId;
                existingEntity.PatientId = entity.PatientId;
                existingEntity.ResponsibilityFunction = entity.ResponsibilityFunction;
                existingEntity.StartDate = entity.StartDate;
                existingEntity.EndDate = entity.EndDate;
                await _repo.Update(existingEntity);
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
            try { var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
