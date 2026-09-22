using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Validators;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Services
{
    /// <summary>
    /// Implementação do <see cref="IViaCepClient"/> usando <see cref="HttpClient"/>.
    /// O HttpClient é fornecido pelo IHttpClientFactory (registrado via AddHttpClient no
    /// projeto de API), não sendo instanciado diretamente nem chamado a partir de Controllers.
    /// </summary>
    public class ViaCepClient : IViaCepClient
    {
        public const string HttpClientName = "ViaCep";

        private readonly HttpClient _httpClient;

        public ViaCepClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ViaCepLookupResult> GetAddressByCepAsync(string? cep, CancellationToken cancellationToken = default)
        {
            // Valida/normaliza o CEP antes de qualquer chamada externa.
            if (!CepValidator.IsValid(cep))
                return ViaCepLookupResult.InvalidCep();

            var normalized = CepValidator.Normalize(cep);

            try
            {
                var address = await _httpClient.GetFromJsonAsync<ViaCepAddressDTO>(
                    $"ws/{normalized}/json/", cancellationToken);

                // CEP inexistente: a ViaCEP responde { "erro": true }.
                if (address == null || address.Erro)
                    return ViaCepLookupResult.NotFound();

                return ViaCepLookupResult.Success(address);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // Timeout da requisição (não foi cancelamento solicitado pelo chamador).
                return ViaCepLookupResult.Unavailable();
            }
            catch (HttpRequestException)
            {
                // API indisponível / erro de rede / status de erro.
                return ViaCepLookupResult.Unavailable();
            }
        }
    }
}
