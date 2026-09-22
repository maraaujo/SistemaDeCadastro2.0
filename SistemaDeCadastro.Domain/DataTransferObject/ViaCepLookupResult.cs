namespace SistemaDeCadastro.Domain.DataTransferObject
{
    /// <summary>
    /// Situação de uma consulta ao ViaCEP, permitindo que o chamador
    /// trate cada caso sem depender de exceções.
    /// </summary>
    public enum ViaCepStatus
    {
        Success,
        InvalidCep,
        NotFound,
        Unavailable
    }

    /// <summary>
    /// Resultado de uma consulta ao ViaCEP. A consulta nunca lança exceção para
    /// indisponibilidade/timeout: retorna <see cref="ViaCepStatus.Unavailable"/>,
    /// de modo que a falha da API externa não impeça o funcionamento do sistema.
    /// </summary>
    public class ViaCepLookupResult
    {
        public ViaCepStatus Status { get; init; }
        public ViaCepAddressDTO? Address { get; init; }

        public bool IsSuccess => Status == ViaCepStatus.Success;

        public static ViaCepLookupResult Success(ViaCepAddressDTO address) =>
            new() { Status = ViaCepStatus.Success, Address = address };

        public static ViaCepLookupResult InvalidCep() =>
            new() { Status = ViaCepStatus.InvalidCep };

        public static ViaCepLookupResult NotFound() =>
            new() { Status = ViaCepStatus.NotFound };

        public static ViaCepLookupResult Unavailable() =>
            new() { Status = ViaCepStatus.Unavailable };
    }
}
