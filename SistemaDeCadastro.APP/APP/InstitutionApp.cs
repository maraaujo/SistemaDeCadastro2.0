using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System.Linq;

namespace SistemaDeCadastro.APP.APP
{
    public class InstitutionApp : IInstitutionApp
    {
        private readonly IInstitutionRepository _repo;

        public InstitutionApp(IInstitutionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> Create(CreateInstitutionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var institution = new Institution
                {
                    Name = entity.Name,
                    Cnpj = entity.Cnpj,
                    Email = entity.Email,
                    Phone = entity.Phone,
                    Active = entity.Active,
                    CreatedAt = DateTime.Now
                };

                await _repo.Create(institution);

                ret.Success = true;
                ret.Message = "Instituição criada com sucesso.";
                ret.Data = institution;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateInstitutionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(i => i.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Instituição não encontrada.";
                    return ret;
                }

                existing.Name = entity.Name;
                existing.Cnpj = entity.Cnpj;
                existing.Email = entity.Email;
                existing.Phone = entity.Phone;
                existing.Active = entity.Active;

                await _repo.Update(existing);

                ret.Success = true;
                ret.Message = "Instituição atualizada com sucesso.";
                ret.Data = existing;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(i => i.Id == id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Instituição não encontrada.";
                    return ret;
                }
                await _repo.Delete(existing);
                ret.Success = true;
                ret.Message = "Instituição removida com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> GetById(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var item = (await _repo.FindBy(i => i.Id == id)).FirstOrDefault();
                ret.Success = true;
                ret.Data = item;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> GetAll()
        {
            var ret = new ApiResponse();
            try
            {
                var items = await _repo.GetAll();
                ret.Success = true;
                ret.Data = items;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }
    }
}
