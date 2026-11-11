using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System.Linq;

namespace SistemaDeCadastro.APP.APP
{
    public class PlanApp : IPlanApp
    {
        private readonly IPlanRepository _repo;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de planos.
        /// </summary>
        public PlanApp(IPlanRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Cadastra um plano de assinatura (nome, descricao, preco mensal, limites de acolhidos e usuarios, ativo)
        /// e devolve o plano criado em Data.
        /// </summary>
        public async Task<ApiResponse> Create(CreatePlanDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var plan = new Plan
                {
                    Name = entity.Name,
                    Description = entity.Description,
                    MonthlyPrice = entity.MonthlyPrice,
                    MaxPatients = entity.MaxPatients,
                    MaxUsers = entity.MaxUsers,
                    Active = entity.Active
                };

                await _repo.Create(plan);
                ret.Success = true;
                ret.Message = "Plano criado com sucesso.";
                ret.Data = plan;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Atualiza todos os dados de um plano existente e devolve-o em Data; retorna "Plano nao encontrado." se nao existir.
        /// </summary>
        public async Task<ApiResponse> Update(UpdatePlanDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Plano não encontrado.";
                    return ret;
                }

                existing.Name = entity.Name;
                existing.Description = entity.Description;
                existing.MonthlyPrice = entity.MonthlyPrice;
                existing.MaxPatients = entity.MaxPatients;
                existing.MaxUsers = entity.MaxUsers;
                existing.Active = entity.Active;

                await _repo.Update(existing);
                ret.Success = true;
                ret.Message = "Plano atualizado com sucesso.";
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
        /// Exclui o plano com o Id informado; retorna "Plano nao encontrado." se nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Plano não encontrado.";
                    return ret;
                }
                await _repo.Delete(existing);
                ret.Success = true;
                ret.Message = "Plano removido com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data o plano com o Id informado (Data fica null, com Success = true, se nao existir).
        /// </summary>
        public async Task<ApiResponse> GetById(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var item = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();
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
        /// Devolve em Data todos os planos cadastrados.
        /// </summary>
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
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }
    }
}
