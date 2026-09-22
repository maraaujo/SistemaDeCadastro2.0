using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Validators;
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
                if (!string.IsNullOrWhiteSpace(entity.Phone) && !PhoneValidator.IsValid(entity.Phone))
                {
                    ret.Success = false;
                    ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                    return ret;
                }

                var responsible = new Responsible
                {
                    Name = entity.Name,
                    Relationship = entity.Relationship,
                    // Armazena apenas os números; a máscara fica para o frontend.
                    Phone = string.IsNullOrWhiteSpace(entity.Phone) ? entity.Phone : PhoneValidator.Normalize(entity.Phone),
                    Address = entity.Address,
                    PatientId = entity.PatientId
                };

                await _responsibleRepository.Create(responsible);
                ret.Success = true;
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
            if (!string.IsNullOrWhiteSpace(entity.Phone) && !PhoneValidator.IsValid(entity.Phone))
            {
                ret.Success = false;
                ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                return ret;
            }

            var existingResponsible = (await _responsibleRepository.FindBy(r => r.Id == entity.Id)).FirstOrDefault();
            if (existingResponsible != null)
            {
                existingResponsible.Name = entity.Name;
                existingResponsible.Relationship = entity.Relationship;
                // Armazena apenas os números; a máscara fica para o frontend.
                existingResponsible.Phone = string.IsNullOrWhiteSpace(entity.Phone) ? entity.Phone : PhoneValidator.Normalize(entity.Phone);
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
