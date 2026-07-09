using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class EmployeeApp : IEmployeeApp
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeApp(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<Employee>> GetAll() => await _employeeRepository.GetAll();

        public async Task<Employee?> GetById(long id) => (await _employeeRepository.FindBy(e => e.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(Employee entity)
        {
            var ret = new ApiResponse();
            try { await _employeeRepository.Create(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Update(Employee entity)
        {
            var ret = new ApiResponse();
            try { await _employeeRepository.Update(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _employeeRepository.FindBy(e => e.Id == id)).FirstOrDefault(); if (entity != null) await _employeeRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
