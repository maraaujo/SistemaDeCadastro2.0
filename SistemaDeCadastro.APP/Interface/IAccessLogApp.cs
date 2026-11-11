using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IAccessLogApp
    {
        /// <summary>Devolve todos os logs de acesso cadastrados.</summary>
        Task<List<AccessLog>> GetAll();
        /// <summary>Devolve o log de acesso do Id informado, ou null se nao existir.</summary>
        Task<AccessLog?> GetById(long id);
        /// <summary>Grava um novo log de acesso (usuario, acao, data/hora, IP, observacao).</summary>
        Task<ApiResponse> Create(CreateAccessLogDTO entity);
        /// <summary>Atualiza os dados de um log de acesso existente.</summary>
        Task<ApiResponse> Update(UpdateAccessLogDTO  entity);
        /// <summary>Exclui o log de acesso do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de logs de acesso conforme o filtro.</summary>
        Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter);
    }
}
