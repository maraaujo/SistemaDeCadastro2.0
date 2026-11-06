namespace SistemaDeCadastro.APP.Interface
{
    // Controle de tentativas de login por e-mail (protecao contra forca bruta).
    public interface ILoginAttemptGuard
    {
        // Tempo restante de bloqueio do e-mail, ou null se o login esta liberado.
        TimeSpan? GetRemainingLockout(string email);

        void RegisterFailure(string email);

        void Reset(string email);
    }
}
