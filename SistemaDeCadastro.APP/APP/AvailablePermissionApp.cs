using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class AvailablePermissionApp : IAvailablePermissionApp
    {
        private readonly IAvailablePermissionRepository _repo;

        public AvailablePermissionApp(IAvailablePermissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AvailablePermission>> GetAll() => await _repo.GetAll();

        public async Task<AvailablePermission?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateAvailablePermissionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
              
                var newAvailablePermission = new AvailablePermission
                {
                    Name = entity.Name,
                    Description = entity.Description,
                    Module = entity.Module,
                    Active = entity.Active
                };
                await _repo.Create(newAvailablePermission);
                ret.Success = true;
                ret.Message = "AvailablePermission criado com successo";
                ret.Data = newAvailablePermission;

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret; 
        }

        public async Task<ApiResponse> Update(UpdateAvailablePermissionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingAvailablePermission = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existingAvailablePermission != null)
                {
                    existingAvailablePermission.Name = entity.Name;
                    existingAvailablePermission.Description = entity.Description;
                    existingAvailablePermission.Module = entity.Module;
                    existingAvailablePermission.Active = entity.Active;
                    await _repo.Update(existingAvailablePermission);
                    ret.Success = true;
                    ret.Message = "AvailablePermission atualizado com successo";
                    ret.Data = existingAvailablePermission;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "AvailablePermission não encontrado";
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
            try { var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
