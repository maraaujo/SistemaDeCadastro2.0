using System;

namespace SistemaDeCadastro.Domain.Utils
{
    public static class DateTimeUtils
    {
        // Resolve o fuso de Sao Paulo tanto em Linux/containers (IANA: "America/Sao_Paulo")
        // quanto no Windows ("E. South America Standard Time"). Sem esse fallback o app
        // quebraria ao iniciar em um dos dois ambientes.
        private static readonly TimeZoneInfo SaoPauloTimeZone = ResolveSaoPauloTimeZone();

        private static TimeZoneInfo ResolveSaoPauloTimeZone()
        {
            foreach (var id in new[] { "America/Sao_Paulo", "E. South America Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }

            // Ultimo recurso: offset fixo de Brasilia (-03:00). O Brasil nao usa mais
            // horario de verao desde 2019, entao isso cobre o cenario atual.
            return TimeZoneInfo.CreateCustomTimeZone(
                "America/Sao_Paulo_Fixed",
                TimeSpan.FromHours(-3),
                "Horario de Brasilia",
                "Horario de Brasilia");
        }

        // Interpreta um DateTime (com Kind Unspecified ou Local) como horario de parede em
        // America/Sao_Paulo e converte para UTC.
        public static DateTime ConvertWallClockToUtc(DateTime wallClock)
        {
            var unspecified = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, SaoPauloTimeZone);
        }

        // Converte UTC para horario de parede em America/Sao_Paulo.
        public static DateTime ConvertUtcToWallClock(DateTime utc)
        {
            var utcKind = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utcKind, SaoPauloTimeZone);
        }

        // "Agora" no horario de parede de Sao Paulo (Kind Unspecified), independente do
        // fuso em que o servidor roda (ex.: UTC no Cloud Run). Use SEMPRE isto no lugar de
        // DateTime.Now para datas/horas ligadas a medicamentos, para que os calculos de
        // atraso e "proxima dose" batam com o horario local do usuario.
        public static DateTime NowSaoPaulo()
        {
            return DateTime.SpecifyKind(ConvertUtcToWallClock(DateTime.UtcNow), DateTimeKind.Unspecified);
        }

        // Offset de Sao Paulo para um horario de parede especifico (-03:00 no cenario atual).
        public static TimeSpan GetSaoPauloOffset(DateTime wallClock)
        {
            var unspecified = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
            return SaoPauloTimeZone.GetUtcOffset(unspecified);
        }

        // Transforma um horario de parede de Sao Paulo em DateTimeOffset com o offset
        // explicito (ex.: "2026-10-14T14:41:00-03:00"). Serializado assim, qualquer cliente
        // calcula o instante correto, sem depender de assumir o fuso.
        public static DateTimeOffset ToSaoPauloOffset(DateTime wallClock)
        {
            var unspecified = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
            return new DateTimeOffset(unspecified, GetSaoPauloOffset(unspecified));
        }
    }
}
