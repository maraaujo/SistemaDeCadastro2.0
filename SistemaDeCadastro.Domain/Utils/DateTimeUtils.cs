using System;

namespace SistemaDeCadastro.Domain.Utils
{
    public static class DateTimeUtils
    {
        private static readonly TimeZoneInfo SaoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        public static DateTime ConvertWallClockToUtc(DateTime wallClock)
        {
            var unspecified = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, SaoPauloTimeZone);
        }

        public static DateTime ConvertUtcToWallClock(DateTime utc)
        {
            var utcKind = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utcKind, SaoPauloTimeZone);
        }
    }
}
