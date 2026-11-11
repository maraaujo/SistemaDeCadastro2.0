using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;
using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.APP
{
    public class ClinicalConditionApp : IClinicalConditionApp
    {
        private readonly IClinicalConditionRepository _clinicalConditionRepository;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de condicoes clinicas.
        /// </summary>
        public ClinicalConditionApp(IClinicalConditionRepository clinicalConditionRepository)
        {
            _clinicalConditionRepository = clinicalConditionRepository;
        }
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve a lista paginada de condicoes clinicas.
        /// </summary>
        public async Task<PagedClinicalConditionDTO> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter)
        {
            return await _clinicalConditionRepository.GetClinicalConditionByFilter(filter);
        }

        /// <summary>
        /// Devolve todas as condicoes clinicas cadastradas.
        /// </summary>
        public async Task<List<ClinicalCondition>> GetAll() => await _clinicalConditionRepository.GetAll();

        /// <summary>
        /// Busca uma condicao clinica pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<ClinicalCondition?> GetById(long id) => (await _clinicalConditionRepository.FindBy(c => c.Id == id)).FirstOrDefault();

        /// <summary>
        /// Cadastra uma condicao clinica (nome, descricao e tipo). Retorna Success = true se gravou.
        /// </summary>
        public async Task<ApiResponse> Create(CreateClinicalConditionDTO clinicalCondition)
        {
            try
            {
                var entity = new ClinicalCondition
                {
                    Name = clinicalCondition.Name,
                    Description = clinicalCondition.Description,
                    Type = clinicalCondition.Type
                };
                await _clinicalConditionRepository.Create(entity);
                return new ApiResponse { Success = true };

            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, ErrorMessage = ApiErrors.From(ex) };
            }
        }

        /// <summary>
        /// Atualiza nome, descricao e tipo de uma condicao clinica existente.
        /// Se o Id nao existir, retorna "Clinical condition not found.".
        /// </summary>
        public async Task<ApiResponse> Update(UpdateClinicalConditionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingEntity = (await _clinicalConditionRepository.FindBy(c => c.Id == entity.Id)).FirstOrDefault();
                if (existingEntity != null)
                {
                    existingEntity.Name = entity.Name;
                    existingEntity.Description = entity.Description;
                    existingEntity.Type = entity.Type;
                    await _clinicalConditionRepository.Update(existingEntity);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Clinical condition not found.";
                }
              

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }
        /// <summary>
        /// Exclui a condicao clinica com o Id informado. Retorna Success = true mesmo se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var entity = (await _clinicalConditionRepository.FindBy(c => c.Id == id)).FirstOrDefault(); if (entity != null) await _clinicalConditionRepository.Delete(entity); ret.Success = true; }
            catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
    }
}
