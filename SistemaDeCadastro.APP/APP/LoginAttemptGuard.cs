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

        public LoginAttemptGuard(IMemoryCache cache)
        {
            _cache = cache;
        }

        private sealed class Attempts
        {
            public int Failures;
            public DateTimeOffset? LockedUntil;
        }

        private static string Key(string email) =>
            "login-attempts:" + (email ?? string.Empty).Trim().ToLowerInvariant();

        public TimeSpan? GetRemainingLockout(string email)
        {
            if (!_cache.TryGetValue(Key(email), out Attempts? attempts) || attempts?.LockedUntil == null)
                return null;

            var remaining = attempts.LockedUntil.Value - DateTimeOffset.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : null;
        }

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

        public void Reset(string email) => _cache.Remove(Key(email));
    }
}
