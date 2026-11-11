using Microsoft.Extensions.Caching.Memory;
using SistemaDeCadastro.APP.Interface;

namespace SistemaDeCadastro.APP.APP
{
    // Bloqueia o e-mail por 15 minutos apos 5 senhas erradas seguidas. As falhas
    // contam mesmo para e-mail inexistente, para o bloqueio nao revelar quais
    // contas existem. O estado fica em memoria: cada instancia da API (Cloud Run)
    // tem o seu e ele zera quando a instancia reinicia.
    public class LoginAttemptGuard : ILoginAttemptGuard
    {
        public const int MaxFailedAttempts = 5;
        public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        private readonly IMemoryCache _cache;

        /// <summary>
        /// Recebe por injecao de dependencia o cache em memoria onde as tentativas ficam guardadas.
        /// </summary>
        public LoginAttemptGuard(IMemoryCache cache)
        {
            _cache = cache;
        }

        private sealed class Attempts
        {
            public int Failures;
            public DateTimeOffset? LockedUntil;
        }

        /// <summary>
        /// Monta a chave do cache para o e-mail (sem espacos e em minusculas, para "A@x.com" e "a@x.com" contarem juntos).
        /// </summary>
        private static string Key(string email) =>
            "login-attempts:" + (email ?? string.Empty).Trim().ToLowerInvariant();

        /// <summary>
        /// Devolve quanto tempo falta para o e-mail ser desbloqueado, ou null se nao estiver bloqueado.
        /// </summary>
        public TimeSpan? GetRemainingLockout(string email)
        {
            if (!_cache.TryGetValue(Key(email), out Attempts? attempts) || attempts?.LockedUntil == null)
                return null;

            var remaining = attempts.LockedUntil.Value - DateTimeOffset.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : null;
        }

        /// <summary>
        /// Conta mais uma senha errada para o e-mail. Ao chegar em 5 falhas, bloqueia o e-mail por 15 minutos
        /// e zera o contador. O registro expira 15 minutos apos a ultima falha.
        /// </summary>
        public void RegisterFailure(string email)
        {
            var key = Key(email);
            var attempts = _cache.GetOrCreate(key, _ => new Attempts())!;

            lock (attempts)
            {
                attempts.Failures++;
                if (attempts.Failures >= MaxFailedAttempts)
                {
                    attempts.LockedUntil = DateTimeOffset.UtcNow.Add(LockoutDuration);
                    attempts.Failures = 0;
                }
            }

            // As falhas "esquecem" 15 minutos depois da ultima tentativa errada.
            _cache.Set(key, attempts, LockoutDuration);
        }

        /// <summary>
        /// Apaga as falhas e o bloqueio do e-mail (chamado apos um login com senha correta).
        /// </summary>
        public void Reset(string email) => _cache.Remove(Key(email));
    }
}
