using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Pageds;
namespace SistemaDeCadastro.APP.APP
{
    public class AccessLogApp : IAccessLogApp
    {
        private readonly IAccessLogRepository _repo;

        /// <summary>
        /// Recebe por injecao de dependencia o repositorio de logs de acesso.
        /// </summary>
        public AccessLogApp(IAccessLogRepository repo)
        {
            _repo = repo;
        }
        /// <summary>
        /// Repassa o filtro ao repositorio e devolve a lista paginada de logs de acesso.
        /// </summary>
        public async Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter)
        {
            return await _repo.GetAccessLogsByFilter(filter);
        }
        // Limite de registros devolvidos por GetAll (os mais recentes). O historico
        // completo e consultado pelo filtro paginado (GetAccessLogsByFilter).
        private const int MaxRecentLogs = 500;

        /// <summary>
        /// Devolve os logs de acesso mais recentes (ate MaxRecentLogs registros).
        /// </summary>
        public async Task<List<AccessLog>> GetAll() => await _repo.GetRecent(MaxRecentLogs);

        /// <summary>
        /// Busca um log de acesso pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<AccessLog?> GetById(long id) => (await _repo.FindBy(a => a.Id == id)).FirstOrDefault();

        /// <summary>
        /// Cria um novo log de acesso (usuario, acao, data/hora, IP e observacao) a partir do DTO.
        /// Retorna Success = true se gravou; em caso de excecao, Success = false com a mensagem em ErrorMessage.
        /// </summary>
        public async Task<ApiResponse> Create(CreateAccessLogDTO entity)
        {
            var ret = new ApiResponse();
            try { 
                var newAccessLog = new AccessLog
                {
                    UserId = entity.UserId,
                    Action = entity.Action,
                  DateTime = entity.DateTime,
                   IpAddress = entity.IpAddress,
                   Observation = entity.Observation
                };
                await _repo.Create(newAccessLog);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }
        

        /// <summary>
        /// Atualiza os dados de um log de acesso existente (busca pelo Id do DTO).
        /// Se o Id nao for encontrado, nada e gravado e o retorno traz "Log de acesso nao encontrado.".
        /// </summary>
        public async Task<ApiResponse> Update(UpdateAccessLogDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingEntity = (await _repo.FindBy(a => a.Id == entity.Id)).FirstOrDefault();
                if (existingEntity != null)
                {
                    existingEntity.UserId = entity.UserId;
                    existingEntity.Action = entity.Action;
                    existingEntity.DateTime = entity.DateTime;
                    existingEntity.IpAddress = entity.IpAddress;
                    existingEntity.Observation = entity.Observation;
                    await _repo.Update(existingEntity);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Log de acesso não encontrado.";
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
        /// Exclui o log de acesso com o Id informado. Retorna Success = false com mensagem de "nao encontrado" se o Id nao existir (ou for de outra instituicao).
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(a => a.Id == id)).FirstOrDefault(); if (e == null) { ret.Success = false; ret.ErrorMessage = "Log de acesso não encontrado."; return ret; } await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
    }
}
