using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.Validators;
namespace SistemaDeCadastro.APP.APP
{
    public class EmployeeApp : IEmployeeApp
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserServiceContext _currentUserService;
        public EmployeeApp(IEmployeeRepository employeeRepository, ICurrentUserServiceContext currentUserService)
        {
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<Employee>> GetAll() => await _employeeRepository.GetAll();

        public async Task<Employee?> GetById(long id) => (await _employeeRepository.FindBy(e => e.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateEmployeeDTO entity)
        {
            var ret = new ApiResponse();
            try { 
                var institutionId = _currentUserService.InstitutionId;

                if (!institutionId.HasValue)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Não foi possível identificar a instituição do usuário logado.";
                    return ret;
                }
                if(entity.DepartmentId == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Departamento do funcionário é obrigatório.";
                    return ret;
                }

                if (!string.IsNullOrWhiteSpace(entity.Cpf) && !CpfValidator.IsValid(entity.Cpf))
                {
                    ret.Success = false;
                    ret.ErrorMessage = CpfValidator.MensagemInvalido;
                    return ret;
                }

                if (!string.IsNullOrWhiteSpace(entity.Phone) && !PhoneValidator.IsValid(entity.Phone))
                {
                    ret.Success = false;
                    ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                    return ret;
                }

                var employee = new Employee
                {
                    Name = entity.Name,
                    // Armazena apenas os números; a máscara fica para o frontend.
                    Cpf = string.IsNullOrWhiteSpace(entity.Cpf) ? entity.Cpf : CpfValidator.Normalize(entity.Cpf),
                    Position = entity.Position,
                    Phone = string.IsNullOrWhiteSpace(entity.Phone) ? entity.Phone : PhoneValidator.Normalize(entity.Phone),
                    Email = entity.Email,
                    AdmissionDate = DateTime.Now,
                    DepartmentId = entity.DepartmentId
                    ,InstitutionId = institutionId.Value
                };
                await _employeeRepository.Create(employee);
                ret.Success = true;
            } catch (Exception ex)
            {
            ret.ErrorMessage = ex.Message;
            ret.Success = false;
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

                if (!string.IsNullOrWhiteSpace(entity.Cpf) && !CpfValidator.IsValid(entity.Cpf))
                {
                    ret.Success = false;
                    ret.ErrorMessage = CpfValidator.MensagemInvalido;
                    return ret;
                }

                if (!string.IsNullOrWhiteSpace(entity.Phone) && !PhoneValidator.IsValid(entity.Phone))
                {
                    ret.Success = false;
                    ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                    return ret;
                }

                existingEmployee.Name = entity.Name;
                // Armazena apenas os números; a máscara fica para o frontend.
                existingEmployee.Cpf = string.IsNullOrWhiteSpace(entity.Cpf) ? entity.Cpf : CpfValidator.Normalize(entity.Cpf);
                existingEmployee.Position = entity.Position;
                existingEmployee.Phone = string.IsNullOrWhiteSpace(entity.Phone) ? entity.Phone : PhoneValidator.Normalize(entity.Phone);
                existingEmployee.Email = entity.Email;
                existingEmployee.DepartmentId = entity.DepartmentId;
                await _employeeRepository.Update(existingEmployee);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.ErrorMessage = ex.Message;
                ret.Success = false;
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
