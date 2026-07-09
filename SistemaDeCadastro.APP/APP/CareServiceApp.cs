using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
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

        public async Task<List<CareService>> GetAll() => await _repo.GetAll();

        public async Task<CareService?> GetById(long id) => (await _repo.FindBy(c => c.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(CareService entity)
        {
            var ret = new ApiResponse();
            try { await _repo.Create(entity); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }

        public async Task<ApiResponse> Update(CareService entity)
        {
            var ret = new ApiResponse();
            try { await _repo.Update(entity); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
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
