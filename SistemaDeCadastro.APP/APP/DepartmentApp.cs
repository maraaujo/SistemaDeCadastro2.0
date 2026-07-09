using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class DepartmentApp : IDepartmentApp
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentApp(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<List<Department>> GetAll() => await _departmentRepository.GetAll();

        public async Task<Department?> GetById(long id) => (await _departmentRepository.FindBy(d => d.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(Department entity)
        {
            var ret = new ApiResponse();
            try { await _departmentRepository.Create(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Update(Department entity)
        {
            var ret = new ApiResponse();
            try { await _departmentRepository.Update(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _departmentRepository.FindBy(d => d.Id == id)).FirstOrDefault(); if (entity != null) await _departmentRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
