using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class UserPermissionApp : IUserPermissionApp
    {
        private readonly IUserPermissionRepository _repo;

        public UserPermissionApp(IUserPermissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UserPermission>> GetAll() => await _repo.GetAll();

        public async Task<UserPermission?> GetById(long id) => (await _repo.FindBy(u => u.Id == id)).FirstOrDefault();

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
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

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
                    ret.ErrorMessage = "UserPermission not found.";
                }

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
                var existingUserPermission = (await _repo.FindBy(u => u.Id == entity.Id)).FirstOrDefault();
            }
            return ret;
        }


        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(u => u.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
