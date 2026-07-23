using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Pageds;
namespace SistemaDeCadastro.APP.APP
{
    public class AccessLogApp : IAccessLogApp
    {
        private readonly IAccessLogRepository _repo;

        public AccessLogApp(IAccessLogRepository repo)
        {
            _repo = repo;
        }
        public async Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter)
        {
            return await _repo.GetAccessLogsByFilter(filter);
        }
        public async Task<List<AccessLog>> GetAll() => await _repo.GetAll();

        public async Task<AccessLog?> GetById(long id) => (await _repo.FindBy(a => a.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateAccessLogDTO entity)
        {
            var ret = new ApiResponse();
            try { 
                var newAccessLog = new AccessLog
                {
                    UserId = entity.UserId,
                    Action = entity.Action,
                  DateTime = entity.DateTime,
                   IpAddress = entity.IpAddress,
                   Observation = entity.Observation
                };
                await _repo.Create(newAccessLog);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }
        

        public async Task<ApiResponse> Update(UpdateAccessLogDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingEntity = (await _repo.FindBy(a => a.Id == entity.Id)).FirstOrDefault();
                if (existingEntity != null)
                {
                    existingEntity.UserId = entity.UserId;
                    existingEntity.Action = entity.Action;
                    existingEntity.DateTime = entity.DateTime;
                    existingEntity.IpAddress = entity.IpAddress;
                    existingEntity.Observation = entity.Observation;
                    await _repo.Update(existingEntity);
                    ret.Success = true;
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
            try { var e = (await _repo.FindBy(a => a.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
