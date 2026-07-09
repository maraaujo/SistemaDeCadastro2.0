using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class ResponsibleApp : IResponsibleApp
    {
        private readonly IResponsibleRepository _responsibleRepository;

        public ResponsibleApp(IResponsibleRepository responsibleRepository)
        {
            _responsibleRepository = responsibleRepository;
        }

        public async Task<List<Responsible>> GetAll() => await _responsibleRepository.GetAll();

        public async Task<Responsible?> GetById(long id) => (await _responsibleRepository.FindBy(r => r.Id == id)).FirstOrDefault();

        public async Task<ApiResponse> Create(Responsible entity)
        {
            var ret = new ApiResponse();
            try
            {
                await _responsibleRepository.Create(entity);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(Responsible entity)
        {
            var ret = new ApiResponse();
            try
            {
                await _responsibleRepository.Update(entity);
                ret.Success = true;
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
            try
            {
                var entity = (await _responsibleRepository.FindBy(r => r.Id == id)).FirstOrDefault();
                if (entity != null) await _responsibleRepository.Delete(entity);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }
    }
}
