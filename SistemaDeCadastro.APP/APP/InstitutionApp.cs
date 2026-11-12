using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Validators;
using SistemaDeCadastro.Infra.Interface;
using System.Linq;

namespace SistemaDeCadastro.APP.APP
{
    public class InstitutionApp : IInstitutionApp
    {
        private readonly IInstitutionRepository _repo;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de instituicoes.
        /// </summary>
        public InstitutionApp(IInstitutionRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Cadastra uma instituicao (nome, CNPJ, e-mail, telefone, ativa). Valida o telefone quando preenchido
        /// e grava so os numeros; CreatedAt recebe a data/hora atual do servidor. Devolve a instituicao em Data.
        /// </summary>
        public async Task<ApiResponse> Create(CreateInstitutionDTO entity)
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

                var institution = new Institution
                {
                    Name = entity.Name,
                    Cnpj = entity.Cnpj,
                    Email = entity.Email,
                    // Armazena apenas os números; a máscara fica para o frontend.
                    Phone = string.IsNullOrWhiteSpace(entity.Phone) ? entity.Phone : PhoneValidator.Normalize(entity.Phone),
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Atualiza nome, CNPJ, e-mail, telefone e status de uma instituicao existente e devolve-a em Data.
        /// Retorna "Instituicao nao encontrada." se o Id nao existir; valida o telefone quando preenchido.
        /// </summary>
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

                if (!string.IsNullOrWhiteSpace(entity.Phone) && !PhoneValidator.IsValid(entity.Phone))
                {
                    ret.Success = false;
                    ret.ErrorMessage = PhoneValidator.MensagemInvalido;
                    return ret;
                }

                existing.Name = entity.Name;
                existing.Cnpj = entity.Cnpj;
                existing.Email = entity.Email;
                // Armazena apenas os números; a máscara fica para o frontend.
                existing.Phone = string.IsNullOrWhiteSpace(entity.Phone) ? entity.Phone : PhoneValidator.Normalize(entity.Phone);
                existing.Active = entity.Active;

                await _repo.Update(existing);

                ret.Success = true;
                ret.Message = "Instituição atualizada com sucesso.";
                ret.Data = existing;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Exclui a instituicao com o Id informado; retorna "Instituicao nao encontrada." se ela nao existir.
        /// </summary>
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a instituicao com o Id informado (Data fica null, com Success = true, se nao existir).
        /// </summary>
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data todas as instituicoes cadastradas.
        /// </summary>
        public async Task<ApiResponse> GetAll()
        {
            var ret = new ApiResponse();
            try
            {
                var items = await _repo.GetAllReadOnly();
                ret.Success = true;
                ret.Data = items;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }
    }
}
