using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.APP
{
    public class SubscriptionApp : ISubscriptionApp
    {
        private readonly ISubscriptionRepository _repo;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de assinaturas.
        /// </summary>
        public SubscriptionApp(ISubscriptionRepository repo)
        {
            _repo = repo;
        }
        /// <summary>
        /// Busca a assinatura ativa da instituicao informada e coloca em Data.
        /// Observacao: a chamada ao repositorio nao e aguardada (sem await), entao Data recebe a Task e nao a assinatura,
        /// e erros da consulta nao caem no catch.
        /// </summary>
        public Task<ApiResponse> GetActiveByInstitutionId(long institutionId)
        {
            var ret = new ApiResponse();
            try
            {
                var item = _repo.GetActiveByInstitutionId(institutionId);
                ret.Success = true;
                ret.Data = item;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return Task.FromResult(ret);
        }
        /// <summary>
        /// Cria uma assinatura da instituicao em um plano, com inicio agora (relogio do servidor), sem data de fim
        /// e status "Ativo", e devolve-a em Data (a mensagem de sucesso fala em "Pagamento de assinatura").
        /// </summary>
        public async Task<ApiResponse> Create(CreateSubscriptionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var payment = new Subscription
                {
                    InstitutionId = entity.InstitutionId,
                    PlanId = entity.PlanId,
                    StartDate = DateTime.Now,
                    EndDate = null,
                    Status = "Ativo",
                    ExternalSubscriptionId = entity.ExternalSubscriptionId
                };

                await _repo.Create(payment);
                ret.Success = true;
                ret.Message = "Pagamento de assinatura criado com sucesso.";
                ret.Data = payment;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Atualiza instituicao, plano, data de fim, status e Id externo de uma assinatura existente e devolve-a em Data.
        /// A data de inicio e sempre sobrescrita com a data/hora atual. Se nao existir, retorna "Pagamento nao encontrado.".
        /// </summary>
        public async Task<ApiResponse> Update(UpdateSubscriptionDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Pagamento não encontrado.";
                    return ret;
                }

                existing.InstitutionId = entity.InstitutionId;
                existing.PlanId = entity.PlanId;
                existing.StartDate = DateTime.Now;
                existing.EndDate =  entity.EndDate;
                existing.Status = entity.Status;
                existing.ExternalSubscriptionId = entity.ExternalSubscriptionId;

                await _repo.Update(existing);
                ret.Success = true;
                ret.Message = "Pagamento atualizado com sucesso.";
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
        /// Exclui a assinatura com o Id informado; retorna "Pagamento nao encontrado." se nao existir.
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
                    ret.ErrorMessage = "Pagamento não encontrado.";
                    return ret;
                }

                await _repo.Delete(existing);
                ret.Success = true;
                ret.Message = "Pagamento removido com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Devolve em Data a assinatura com o Id informado (Data fica null, com Success = true, se nao existir).
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
        /// Devolve em Data todas as assinaturas cadastradas.
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

