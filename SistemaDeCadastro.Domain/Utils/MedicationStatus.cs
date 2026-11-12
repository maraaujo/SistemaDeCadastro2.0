using System;
using System.Linq;

namespace SistemaDeCadastro.Domain.Utils
{
    // Regra unica para interpretar o status (texto livre) de uma administracao de
    // medicamento. Usada no registro, nos lembretes, no relatorio do dia e no painel
    // administrativo, para que todos decidam "a dose foi dada?" da mesma forma.
    public static class MedicationStatus
    {
        // Status (pt-BR ou en) que indicam que a dose foi efetivamente administrada.
        public static readonly string[] Administered =
        {
            "Administrado", "Administrada", "Realizado", "Realizada",
            "Ministrado", "Ministrada", "Administered"
        };

        // Status que indicam que a dose NAO foi administrada (recusa, cancelamento...).
        public static readonly string[] NotAdministered =
        {
            "Não administrado", "Nao administrado", "Não administrada", "Nao administrada",
            "Cancelado", "Cancelada", "NotAdministered", "Not administered",
            "Canceled", "Cancelled"
        };

        public static bool IsAdministered(string? status) =>
            !string.IsNullOrWhiteSpace(status) &&
            Administered.Any(s => s.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));

        public static bool IsNotAdministered(string? status) =>
            !string.IsNullOrWhiteSpace(status) &&
            NotAdministered.Any(s => s.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));

        // A dose conta como dada quando o status nao a marca como "nao administrada" e
        // ha horario de administracao registrado (ou o status diz que foi administrada).
        public static bool WasGiven(string? status, DateTime? administeredDateTime) =>
            !IsNotAdministered(status) &&
            (administeredDateTime != null || IsAdministered(status));
    }
}
