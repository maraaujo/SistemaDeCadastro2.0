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

        /// <summary>
        /// True quando a consulta encontrou o endereco (Status = Success).
        /// </summary>
        public bool IsSuccess => Status == ViaCepStatus.Success;

        /// <summary>
        /// Cria um resultado de sucesso contendo o endereco retornado pelo ViaCEP.
        /// </summary>
        public static ViaCepLookupResult Success(ViaCepAddressDTO address) =>
            new() { Status = ViaCepStatus.Success, Address = address };

        /// <summary>
        /// Cria um resultado indicando que o CEP informado e invalido (sem endereco).
        /// </summary>
        public static ViaCepLookupResult InvalidCep() =>
            new() { Status = ViaCepStatus.InvalidCep };

        /// <summary>
        /// Cria um resultado indicando que o CEP e valido mas nao existe no ViaCEP.
        /// </summary>
        public static ViaCepLookupResult NotFound() =>
            new() { Status = ViaCepStatus.NotFound };

        /// <summary>
        /// Cria um resultado indicando que o ViaCEP esta indisponivel (erro de rede ou timeout).
        /// </summary>
        public static ViaCepLookupResult Unavailable() =>
            new() { Status = ViaCepStatus.Unavailable };
    }
}
