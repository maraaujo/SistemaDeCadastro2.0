using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Domain.Utils;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    // Limpezas em massa do administrador da plataforma. Todas as consultas usam
    // IgnoreQueryFilters: o alcance e global (todas as instituicoes), por isso o acesso
    // fica restrito a policy SaasAdmin no controller e a checagem IsPlatformAdmin no App.
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia.</summary>
        public MaintenanceRepository(SistemaDeCadastroContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Conta os registros de cada tabela operacional, em todas as instituicoes (um COUNT por tabela).
        /// </summary>
        public async Task<MaintenanceCountsDTO> GetSummary()
        {
            return new MaintenanceCountsDTO
            {
                Patients = await _context.Patients.IgnoreQueryFilters().CountAsync(),
                Responsibles = await _context.Responsibles.IgnoreQueryFilters().CountAsync(),
                PatientClinicalConditions = await _context.PatientClinicalConditions.IgnoreQueryFilters().CountAsync(),
                ScheduledMedicines = await _context.MedicinePatientClinicalConditions.IgnoreQueryFilters().CountAsync(),
                MedicationAdministrations = await _context.MedicationAdministrations.IgnoreQueryFilters().CountAsync(),
                Appointments = await _context.Appointments.IgnoreQueryFilters().CountAsync(),
                Payments = await _context.Payments.IgnoreQueryFilters().CountAsync(),
                PatientEmployees = await _context.PatientEmployees.IgnoreQueryFilters().CountAsync(),
                Employees = await _context.Employees.IgnoreQueryFilters().CountAsync(),
                Departments = await _context.Departments.IgnoreQueryFilters().CountAsync(),
                Medicines = await _context.Medicines.IgnoreQueryFilters().CountAsync()
            };
        }

        /// <summary>
        /// Apaga todos os acolhidos e seus dependentes, de todas as instituicoes, em uma unica transacao,
        /// e registra a operacao na trilha de auditoria.
        /// </summary>
        public async Task<MaintenanceCountsDTO> DeleteAllPatients(long? userId, string? ipAddress)
        {
            var removed = new MaintenanceCountsDTO();

            await ExecuteInTransaction(async () =>
            {
                await DeletePatientsAndDependents(removed);

                await LogCleanup(userId, ipAddress,
                    "Excluiu todos os acolhidos (limpeza global)", removed);
            });

            return removed;
        }

        /// <summary>
        /// Zera os dados operacionais (acolhidos e dependentes, funcionarios, departamentos e medicamentos)
        /// de todas as instituicoes, em uma unica transacao, e registra a operacao na trilha de auditoria.
        /// </summary>
        public async Task<MaintenanceCountsDTO> ResetSystemData(long? userId, string? ipAddress)
        {
            var removed = new MaintenanceCountsDTO();

            await ExecuteInTransaction(async () =>
            {
                await DeletePatientsAndDependents(removed);

                // So depois dos acolhidos: administracoes, medicamentos programados e vinculos
                // referenciam funcionarios; funcionarios referenciam departamentos; e os
                // medicamentos programados referenciam os medicamentos.
                removed.Employees = await DeleteAll(_context.Employees);
                removed.Departments = await DeleteAll(_context.Departments);
                removed.Medicines = await DeleteAll(_context.Medicines);

                await LogCleanup(userId, ipAddress,
                    "Zerou os dados do sistema (limpeza global)", removed);
            });

            return removed;
        }

        // Ordem das exclusoes: sempre os filhos antes dos pais, porque todas as chaves
        // estrangeiras do banco sao Restrict (o banco recusa apagar um pai com filhos).
        private async Task DeletePatientsAndDependents(MaintenanceCountsDTO removed)
        {
            foreach (var table in LegacyPatientTables)
                await DeleteLegacyTableIfExists(table);

            removed.MedicationAdministrations = await DeleteAll(_context.MedicationAdministrations);
            removed.ScheduledMedicines = await DeleteAll(_context.MedicinePatientClinicalConditions);
            removed.PatientClinicalConditions = await DeleteAll(_context.PatientClinicalConditions);
            removed.Payments = await DeleteAll(_context.Payments);
            removed.Appointments = await DeleteAll(_context.Appointments);
            removed.PatientEmployees = await DeleteAll(_context.PatientEmployees);
            removed.Responsibles = await DeleteAll(_context.Responsibles);
            removed.Patients = await DeleteAll(_context.Patients);
        }

        // Tabelas da primeira versao do banco (atendimentos e doencas do acolhido) que nao sao
        // mais usadas pelo sistema nem mapeadas no contexto, mas que, se ainda existirem,
        // referenciam acolhidos e agendamentos por chave estrangeira. Com linhas antigas
        // nelas o banco recusaria apagar os acolhidos, entao sao esvaziadas primeiro.
        private static readonly string[] LegacyPatientTables = { "atendimentos", "acolhido_doenca" };

        // So existe no banco real (MySQL). O nome da tabela vem sempre da lista fixa acima,
        // nunca de dados do usuario.
        private async Task DeleteLegacyTableIfExists(string table)
        {
            if (!_context.Database.IsRelational())
                return;

            var exists = await _context.Database
                .SqlQueryRaw<int>(
                    "SELECT COUNT(*) AS `Value` FROM information_schema.tables " +
                    "WHERE table_schema = DATABASE() AND table_name = {0}", table)
                .SingleAsync() > 0;

            if (exists)
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM `" + table + "`");
        }

        // Apaga todas as linhas da tabela, de todas as instituicoes, e devolve quantas eram.
        // No banco real e um unico DELETE (ExecuteDeleteAsync). O provedor em memoria dos
        // testes nao suporta esse comando, entao la as linhas sao carregadas e removidas.
        private async Task<int> DeleteAll<T>(DbSet<T> set) where T : class
        {
            var query = set.IgnoreQueryFilters();

            if (_context.Database.IsRelational())
                return await query.ExecuteDeleteAsync();

            var rows = await query.ToListAsync();
            set.RemoveRange(rows);
            await _context.SaveChangesAsync();
            return rows.Count;
        }

        // ExecuteDelete nao passa pela auditoria automatica do SaveChanges: grava
        // explicitamente em log_acesso quem fez a limpeza e quanto foi apagado.
        private async Task LogCleanup(long? userId, string? ipAddress, string action, MaintenanceCountsDTO removed)
        {
            if (!userId.HasValue)
                return;

            _context.AccessLogs.Add(new AccessLog
            {
                UserId = userId.Value,
                Action = action,
                DateTime = DateTimeUtils.NowSaoPaulo(),
                IpAddress = ipAddress ?? string.Empty,
                Observation =
                    $"Removidos de todas as instituições: {removed.Patients} acolhido(s), " +
                    $"{removed.Responsibles} responsável(is), {removed.PatientClinicalConditions} condição(ões) clínica(s) de acolhidos, " +
                    $"{removed.ScheduledMedicines} medicamento(s) programado(s), {removed.MedicationAdministrations} administração(ões), " +
                    $"{removed.Appointments} agendamento(s), {removed.Payments} pagamento(s), " +
                    $"{removed.PatientEmployees} vínculo(s) acolhido-funcionário, {removed.Employees} funcionário(s), " +
                    $"{removed.Departments} departamento(s) e {removed.Medicines} medicamento(s)."
            });

            await _context.SaveChangesAsync();
        }

        // Tudo ou nada: se qualquer exclusao falhar, as anteriores sao desfeitas.
        private async Task ExecuteInTransaction(Func<Task> action)
        {
            // O provedor em memoria (testes) nao tem transacao; e se ja houver uma
            // aberta, as exclusoes simplesmente participam dela.
            if (!_context.Database.IsRelational() || _context.Database.CurrentTransaction != null)
            {
                await action();
                return;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            await action();
            await transaction.CommitAsync();
        }
    }
}
