using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Utils;

namespace SistemaDeCadastro.Domain.SistemaCadastroContext;

// Auditoria automatica: toda criacao, alteracao ou exclusao salva por um usuario
// logado gera um registro em log_acesso com quem fez, o que fez, quando, de qual
// IP e quais campos mudaram (valor antigo -> novo). Fica centralizado aqui para
// valer para todas as telas, sem cada App precisar lembrar de gravar log.
public partial class SistemaDeCadastroContext
{
    private static readonly Dictionary<string, string> AuditEntityNames = new()
    {
        [nameof(Patient)] = "Acolhido",
        [nameof(Responsible)] = "Responsável",
        [nameof(Employee)] = "Funcionário",
        [nameof(Department)] = "Departamento",
        [nameof(MedicationAdministration)] = "Administração de medicamento",
        [nameof(ClinicalCondition)] = "Condição clínica",
        [nameof(PatientClinicalCondition)] = "Condição clínica do acolhido",
        [nameof(BloodType)] = "Tipo sanguíneo",
        [nameof(Medicine)] = "Medicamento",
        [nameof(MedicinePatientClinicalCondition)] = "Medicamento programado",
        [nameof(PatientEmployee)] = "Vínculo acolhido-funcionário",
        [nameof(LoginAccount)] = "Conta de acesso",
        [nameof(AvailablePermission)] = "Permissão",
        [nameof(UserPermission)] = "Permissão do usuário",
        [nameof(Appointment)] = "Agendamento",
        [nameof(Payment)] = "Pagamento",
        [nameof(Institution)] = "Instituição",
        [nameof(Plan)] = "Plano",
        [nameof(Subscription)] = "Assinatura",
        [nameof(SubscriptionPayment)] = "Pagamento de assinatura",
    };

    // Campos cujo VALOR nunca vai para o log (so registra que foram alterados).
    private static readonly string[] AuditSensitiveFragments = { "password", "senha", "hash", "token", "secret" };

    private static readonly CultureInfo AuditCulture = CultureInfo.GetCultureInfo("pt-BR");
    private const int AuditMaxValueLength = 200;
    private const int AuditMaxObservationLength = 4000;

    private sealed class PendingAudit
    {
        public required EntityEntry Entry { get; init; }
        public required EntityState State { get; init; }
        public string? KeyBeforeSave { get; init; }
        public string Details { get; init; } = string.Empty;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var pending = CollectPendingAudits(acceptAllChangesOnSuccess);
        var result = base.SaveChanges(acceptAllChangesOnSuccess);

        if (pending.Count > 0)
        {
            var logs = AddAuditLogs(pending);
            try
            {
                base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (Exception ex)
            {
                DiscardAuditLogs(logs, ex);
            }
        }

        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var pending = CollectPendingAudits(acceptAllChangesOnSuccess);
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        if (pending.Count > 0)
        {
            var logs = AddAuditLogs(pending);
            try
            {
                await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (Exception ex)
            {
                DiscardAuditLogs(logs, ex);
            }
        }

        return result;
    }

    // Antes de salvar: guarda o que mudou (depois do save os valores originais
    // se perdem e as entidades excluidas saem do ChangeTracker).
    private List<PendingAudit> CollectPendingAudits(bool acceptAllChangesOnSuccess)
    {
        // Sem usuario logado (ex.: o proprio login) nao ha quem registrar. Com
        // acceptAllChangesOnSuccess=false o segundo SaveChanges regravaria as
        // alteracoes, entao a auditoria e ignorada nesse caso (nao usado no sistema).
        if (_currentUserService?.UserId == null || !acceptAllChangesOnSuccess)
            return new List<PendingAudit>();

        ChangeTracker.DetectChanges();

        var pending = new List<PendingAudit>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AccessLog || entry.Metadata.FindPrimaryKey() == null)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    pending.Add(new PendingAudit { Entry = entry, State = entry.State, Details = DescribeIdentity(entry) });
                    break;
                case EntityState.Modified:
                    pending.Add(new PendingAudit { Entry = entry, State = entry.State, KeyBeforeSave = FormatKey(entry), Details = DescribeChanges(entry) });
                    break;
                case EntityState.Deleted:
                    pending.Add(new PendingAudit { Entry = entry, State = entry.State, KeyBeforeSave = FormatKey(entry), Details = DescribeIdentity(entry) });
                    break;
            }
        }

        return pending;
    }

    // Depois de salvar: monta os registros (o Id de quem foi criado ja existe).
    private List<AccessLog> AddAuditLogs(List<PendingAudit> pending)
    {
        var userId = _currentUserService!.UserId!.Value;
        var ip = _currentUserService.IpAddress ?? string.Empty;
        var now = DateTimeUtils.NowSaoPaulo();

        var logs = pending.Select(p =>
        {
            var key = p.State == EntityState.Added ? FormatKey(p.Entry) : p.KeyBeforeSave;
            var verb = p.State switch
            {
                EntityState.Added => "Criou",
                EntityState.Modified => "Alterou",
                _ => "Excluiu",
            };
            var action = $"{verb} {EntityLabel(p.Entry)}" + (string.IsNullOrEmpty(key) ? string.Empty : $" #{key}");

            return new AccessLog
            {
                UserId = userId,
                Action = Truncate(action, 255),
                DateTime = now,
                IpAddress = ip,
                Observation = Truncate(p.Details, AuditMaxObservationLength),
            };
        }).ToList();

        AccessLogs.AddRange(logs);
        return logs;
    }

    // Se gravar o log falhar, a alteracao do usuario (ja salva) nao pode ser
    // desfeita nem gerar erro na tela: descarta os logs para nao ficarem presos
    // no ChangeTracker e serem reenviados no proximo SaveChanges.
    private void DiscardAuditLogs(IEnumerable<AccessLog> logs, Exception ex)
    {
        foreach (var log in logs)
            Entry(log).State = EntityState.Detached;

        Console.WriteLine($"[Auditoria] Falha ao gravar log_acesso: {ex.GetBaseException().Message}");
    }

    private static string DescribeChanges(EntityEntry entry)
    {
        var parts = new List<string>();

        foreach (var prop in entry.Properties)
        {
            if (!prop.IsModified || Equals(prop.OriginalValue, prop.CurrentValue))
                continue;

            var name = prop.Metadata.Name;
            parts.Add(IsSensitive(name)
                ? $"{name}: (alterado)"
                : $"{name}: {FormatValue(prop.OriginalValue)} → {FormatValue(prop.CurrentValue)}");
        }

        return parts.Count == 0 ? "Nenhum valor alterado." : string.Join("; ", parts);
    }

    // Identificacao legivel de um registro criado/excluido (ex.: "Name: Felipe").
    private static string DescribeIdentity(EntityEntry entry)
    {
        var nameProp = entry.Properties.FirstOrDefault(p =>
            p.Metadata.Name is "Name" or "Nome" or "Email");

        return nameProp == null ? string.Empty : $"{nameProp.Metadata.Name}: {FormatValue(nameProp.CurrentValue)}";
    }

    private static string? FormatKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key == null)
            return null;

        return string.Join("/", key.Properties.Select(p => Convert.ToString(entry.Property(p.Name).CurrentValue, AuditCulture)));
    }

    private static string EntityLabel(EntityEntry entry) =>
        AuditEntityNames.TryGetValue(entry.Metadata.ClrType.Name, out var label) ? label : entry.Metadata.ClrType.Name;

    private static bool IsSensitive(string propertyName) =>
        AuditSensitiveFragments.Any(f => propertyName.Contains(f, StringComparison.OrdinalIgnoreCase));

    private static string FormatValue(object? value) => value switch
    {
        null => "(vazio)",
        string s when string.IsNullOrWhiteSpace(s) => "(vazio)",
        string s => Truncate(s, AuditMaxValueLength),
        bool b => b ? "Sim" : "Não",
        DateTime dt => dt.TimeOfDay == TimeSpan.Zero ? dt.ToString("dd/MM/yyyy", AuditCulture) : dt.ToString("dd/MM/yyyy HH:mm", AuditCulture),
        DateTimeOffset dto => dto.ToString("dd/MM/yyyy HH:mm", AuditCulture),
        TimeSpan ts => ts.ToString(@"hh\:mm", AuditCulture),
        _ => Truncate(Convert.ToString(value, AuditCulture) ?? string.Empty, AuditMaxValueLength),
    };

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..(max - 1)] + "…";
}
