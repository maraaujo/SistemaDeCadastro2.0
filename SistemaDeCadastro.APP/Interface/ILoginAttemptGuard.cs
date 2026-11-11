namespace SistemaDeCadastro.APP.Interface
{
    // Controle de tentativas de login por e-mail (protecao contra forca bruta).
    public interface ILoginAttemptGuard
    {
        // Tempo restante de bloqueio do e-mail, ou null se o login esta liberado.
        TimeSpan? GetRemainingLockout(string email);

        /// <summary>Registra uma senha errada para o e-mail (bloqueia apos atingir o limite de falhas).</summary>
        void RegisterFailure(string email);

        /// <summary>Zera as falhas e o bloqueio do e-mail (usado apos login com senha correta).</summary>
        void Reset(string email);
    }
}
