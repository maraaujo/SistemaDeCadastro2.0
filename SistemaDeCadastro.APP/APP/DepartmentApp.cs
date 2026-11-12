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

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de departamentos e o usuario logado.
        /// </summary>
        public DepartmentApp(IDepartmentRepository departmentRepository, ICurrentUserServiceContext currentUserService)
        {
            _departmentRepository = departmentRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Devolve todos os departamentos visiveis para a instituicao do usuario.
        /// </summary>
        public async Task<List<Department>> GetAll() => await _departmentRepository.GetAllReadOnly();

        /// <summary>
        /// Busca um departamento pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<Department?> GetById(long id) => (await _departmentRepository.FindBy(d => d.Id == id)).FirstOrDefault();

        /// <summary>
        /// Cria um departamento (nome e descricao) na instituicao do usuario logado.
        /// Recusa se nao for possivel identificar a instituicao do usuario.
        /// </summary>
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Atualiza nome e descricao de um departamento existente. Se o Id nao existir, retorna "Department not found.".
        /// </summary>
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
                    ret.ErrorMessage = "Departamento não encontrado.";
                }
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Exclui o departamento com o Id informado. Retorna Success = true mesmo se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _departmentRepository.FindBy(d => d.Id == id)).FirstOrDefault(); if (entity != null) await _departmentRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve a lista paginada de departamentos.
        /// </summary>
        public async Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter)
        {
            return await _departmentRepository.GetDepartmentByFilter(filter);
        }
    }
}
