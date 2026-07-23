using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Pageds;  
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

        public async Task<ApiResponse> Create(CreateEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try { 
                if(entity.DepartmentId == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Departamento do funcionário é obrigatório.";
                    return ret;
                }
                var employee = new Employee
                {
                    Name = entity.Name,
                    Cpf = entity.Cpf,
                    Position = entity.Position,
                    Phone = entity.Phone,
                    Email = entity.Email,
                    AdmissionDate = DateTime.Now,
                    DepartmentId = entity.DepartmentId
                };
            } catch (Exception ex) 
            {
            ret.ErrorMessage = ex.Message;
            ret.Success = true;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingEmployee = (await _employeeRepository.FindBy(e => e.Id == entity.Id)).FirstOrDefault();
                if(existingEmployee == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Funcionário não encontrado.";
                    return ret;
                }   
                existingEmployee.Name = entity.Name;
                existingEmployee.Cpf = entity.Cpf;
                existingEmployee.Position = entity.Position;
                existingEmployee.Phone = entity.Phone;
                existingEmployee.Email = entity.Email;
                existingEmployee.DepartmentId = entity.DepartmentId;
                await _employeeRepository.Update(existingEmployee);

            }
            catch (Exception ex)
            {
                ret.ErrorMessage = ex.Message;
                ret.Success = true;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _employeeRepository.FindBy(e => e.Id == id)).FirstOrDefault(); if (entity != null) await _employeeRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
        public async Task<PagedEmployeeDTO> GetEmployeeByFilter(EmployeeFilterDTO filter)
        {
            return await _employeeRepository.GetEmployeeByFilter(filter);
        }
    }
}
