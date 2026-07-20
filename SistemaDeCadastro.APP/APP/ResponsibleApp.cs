using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
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

        public async Task<ApiResponse> Create(CreateResponsibleDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var responsible = new Responsible
                {
                    Name = entity.Name,
                    Relationship = entity.Relationship,
                    Phone = entity.Phone,
                    Address = entity.Address,
                    PatientId = entity.PatientId
                };
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateResponsibleDTO entity)
        {
            var ret = new ApiResponse();
            try { 
            var existingResponsible = (await _responsibleRepository.FindBy(r => r.Id == entity.Id)).FirstOrDefault();
            if (existingResponsible != null)
            {
                existingResponsible.Name = entity.Name;
                existingResponsible.Relationship = entity.Relationship;
                existingResponsible.Phone = entity.Phone;
                existingResponsible.Address = entity.Address;
                existingResponsible.PatientId = entity.PatientId;
                await _responsibleRepository.Update(existingResponsible);
                ret.Success = true;
            }
            else
            {
                ret.Success = false;
                ret.ErrorMessage = "Responsible not found.";
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
