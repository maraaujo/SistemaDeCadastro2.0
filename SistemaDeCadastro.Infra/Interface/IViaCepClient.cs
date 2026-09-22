using System.Threading;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.DataTransferObject;

namespace SistemaDeCadastro.Infra.Interface
{
    /// <summary>
    /// Client de integração com a API pública ViaCEP.
    /// </summary>
    public interface IViaCepClient
    {
        /// <summary>
        /// Consulta o endereço de um CEP. Aceita CEP com ou sem máscara.
        /// Nunca lança exceção por indisponibilidade/timeout da API externa:
        /// nesses casos retorna <see cref="ViaCepStatus.Unavailable"/>.
        /// </summary>
        Task<ViaCepLookupResult> GetAddressByCepAsync(string? cep, CancellationToken cancellationToken = default);
    }
}
