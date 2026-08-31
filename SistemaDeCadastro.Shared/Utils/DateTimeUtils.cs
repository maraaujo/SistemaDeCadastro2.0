using System;

namespace SistemaDeCadastro.Shared.Utils
{
    public static class DateTimeUtils
    {
        private static readonly TimeZoneInfo SaoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        // Interpreta um DateTime (com Kind Unspecified ou Local) como horário de parede em America/Sao_Paulo e converte para UTC
        public static DateTime ConvertWallClockToUtc(DateTime wallClock)
        {
            var unspecified = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, SaoPauloTimeZone);
        }

        // Converte UTC para horário de parede em America/Sao_Paulo
        public static DateTime ConvertUtcToWallClock(DateTime utc)
        {
            var utcKind = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utcKind, SaoPauloTimeZone);
        }
    }
}
