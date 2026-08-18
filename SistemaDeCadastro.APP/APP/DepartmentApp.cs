using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class DepartmentApp : IDepartmentApp
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICurrentUserServiceContext _currentUserService;

        public DepartmentApp(IDepartmentRepository departmentRepository, ICurrentUserServiceContext currentUserService)
        {
            _departmentRepository = departmentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<Department>> GetAll() => await _departmentRepository.GetAll();

        public async Task<Department?> GetById(long id) => (await _departmentRepository.FindBy(d => d.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateDepartmentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var institutionId = _currentUserService.InstitutionId;

                if (!institutionId.HasValue)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Não foi possível identificar a instituição do usuário logado.";
                    return ret;
                }
                var department = new Department
                {
                    Name = entity.Name,
                    Description = entity.Description
                    ,InstitutionId = institutionId.Value
                };
                await _departmentRepository.Create(department);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateDepartmentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingDepartment = (await _departmentRepository.FindBy(d => d.Id == entity.Id)).FirstOrDefault();
                if (existingDepartment != null) {
                    existingDepartment.Name = entity.Name;
                    existingDepartment.Description = entity.Description;
                    await _departmentRepository.Update(existingDepartment);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Department not found.";
                }
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
            try { var entity = (await _departmentRepository.FindBy(d => d.Id == id)).FirstOrDefault(); if (entity != null) await _departmentRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
        public async Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter)
        {
            return await _departmentRepository.GetDepartmentByFilter(filter);
        }
    }
}
