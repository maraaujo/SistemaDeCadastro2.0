using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class PatientEmployeeApp : IPatientEmployeeApp
    {
        private readonly IPatientEmployeeRepository _repo;
        private readonly IPatientRepository _patientRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public PatientEmployeeApp(IPatientEmployeeRepository repo, IPatientRepository patientRepository, IEmployeeRepository employeeRepository)
        {
            _repo = repo;
            _patientRepository = patientRepository;
            _employeeRepository = employeeRepository;
        }

        // Acolhido e funcionario vem do cliente: os dois precisam ser da instituicao
        // do usuario (os repositorios ja aplicam o filtro de instituicao).
        private async Task<string?> ValidateReferences(long patientId, long employeeId)
        {
            if (!await _patientRepository.Any(p => p.Id == patientId))
                return "Acolhido não encontrado para a instituição do usuário logado.";
            if (!await _employeeRepository.Any(e => e.Id == employeeId))
                return "Funcionário não encontrado para a instituição do usuário logado.";
            return null;
        }
        public async Task<PagedPatientEmployeeDTO> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter)
        {
            return await _repo.GetPagedPatientEmployeeByFilter(filter);
        }

        public async Task<List<PatientEmployee>> GetAll() => await _repo.GetAll();

        public async Task<PatientEmployee?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreatePatientEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try { 
            var referenceError = await ValidateReferences(entity.PatientId, entity.EmployeeId);
            if (referenceError != null)
            {
                ret.Success = false;
                ret.ErrorMessage = referenceError;
                return ret;
            }

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
            ret.ErrorMessage = ApiErrors.From(ex);
        }
        return ret;
    }

        public async Task<ApiResponse> Update(UpdatePatientEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try { 
            var existingEntity = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existingEntity == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Vínculo não encontrado.";
                    return ret;
                }

                var referenceError = await ValidateReferences(entity.PatientId, entity.EmployeeId);
                if (referenceError != null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = referenceError;
                    return ret;
                }

                existingEntity.EmployeeId = entity.EmployeeId;
                existingEntity.PatientId = entity.PatientId;
                existingEntity.ResponsibilityFunction = entity.ResponsibilityFunction;
                existingEntity.StartDate = entity.StartDate;
                existingEntity.EndDate = entity.EndDate;
                await _repo.Update(existingEntity);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
    }
}
