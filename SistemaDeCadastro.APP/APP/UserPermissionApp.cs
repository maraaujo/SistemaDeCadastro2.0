using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class UserPermissionApp : IUserPermissionApp
    {
        private readonly IUserPermissionRepository _repo;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de permissoes de usuario.
        /// </summary>
        public UserPermissionApp(IUserPermissionRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Devolve todas as permissoes concedidas a usuarios.
        /// </summary>
        public async Task<List<UserPermission>> GetAll() => await _repo.GetAllReadOnly();

        /// <summary>
        /// Busca uma permissao de usuario pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<UserPermission?> GetById(long id) => (await _repo.FindBy(u => u.Id == id)).FirstOrDefault();

        /// <summary>
        /// Concede uma permissao a um usuario (Id, usuario, permissao e data da concessao vindos do DTO).
        /// </summary>
        public async Task<ApiResponse> Create(CreateUserPermissionDTO entity)
        {
            var ret = new ApiResponse();
            try { 
            var newUser = new UserPermission
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PermissionId = entity.PermissionId,
                GrantedAt = entity.GrantedAt
            };
            await _repo.Create(newUser);
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
        /// Atualiza usuario, permissao e data da concessao de uma permissao existente ("UserPermission not found." se nao existir).
        /// No catch, apos preencher o erro, ainda refaz a busca no repositorio sem usar o resultado.
        /// </summary>
        public async Task<ApiResponse> Update(UpdateUserPermissionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingUserPermission = (await _repo.FindBy(u => u.Id == entity.Id)).FirstOrDefault();
                if (existingUserPermission != null)
                {
                    existingUserPermission.UserId = entity.UserId;
                    existingUserPermission.PermissionId = entity.PermissionId;
                    existingUserPermission.GrantedAt = entity.GrantedAt;
                    await _repo.Update(existingUserPermission);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Permissão do usuário não encontrada.";
                }

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
                var existingUserPermission = (await _repo.FindBy(u => u.Id == entity.Id)).FirstOrDefault();
            }
            return ret;
        }


        /// <summary>
        /// Exclui a permissao de usuario com o Id informado. Retorna Success = true mesmo se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(u => u.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
    }
}
