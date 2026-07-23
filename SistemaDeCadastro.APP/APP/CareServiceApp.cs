using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class CareServiceApp : ICareServiceApp
    {
        private readonly ICareServiceRepository _repo;

        public CareServiceApp(ICareServiceRepository repo)
        {
            _repo = repo;
        }
        public async Task<PagedCareServiceDTO> GetCareServiceByFilter(CareServiceFilterDTO filter) => await _repo.GetCareServiceByFilter(filter);
        public async Task<List<CareService>> GetAll() => await _repo.GetAll();

        public async Task<CareService?> GetById(long id) => (await _repo.FindBy(c => c.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CreateCareServiceDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var careService = new CareService
                {
                    AppointmentId = entity.AppointmentId,
                    UserId = entity.UserId,
                    Description = entity.Description,
                    ServiceDate = entity.ServiceDate,
                    Referral = entity.Referral,
                    Observations = entity.Observations
                };
                await _repo.Create(careService);
                ret.Success = true;

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;

            }
            return ret;
        }
        public async Task<ApiResponse> Update(UpdateCareServiceDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingCareService = (await _repo.FindBy(c => c.Id == entity.Id)).FirstOrDefault();
                if (existingCareService != null)
                {
                    existingCareService.AppointmentId = entity.AppointmentId;
                    existingCareService.UserId = entity.UserId;
                    existingCareService.Description = entity.Description;
                    existingCareService.ServiceDate = entity.ServiceDate;
                    existingCareService.Referral = entity.Referral;
                    existingCareService.Observations = entity.Observations;
                    await _repo.Update(existingCareService);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "CareService not found.";
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
            try { var e = (await _repo.FindBy(c => c.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
