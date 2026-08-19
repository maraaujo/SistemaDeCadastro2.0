using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    // Repositorio de leitura do back-office. Todas as consultas usam AsNoTracking e,
    // quando precisam de visao global (idosos, doses), IgnoreQueryFilters para
    // ultrapassar o filtro de tenant -- essa e a unica visao que enxerga todas as
    // instituicoes, e ela so fica acessivel ao administrador do SaaS (ver controller).
    // Nenhuma alteracao estrutural no banco: usa as tabelas existentes.
    public class AdminOverviewRepository : BaseRepository<Institution>, IAdminOverviewRepository
    {
        private readonly SistemaDeCadastroContext _context;

        // Tolerancia (minutos) para considerar uma dose administrada "no prazo".
        private const int OnTimeToleranceMinutes = 60;

        // Situacoes reais equivalentes, normalizadas para os codigos apresentados.
        private static readonly string[] ActiveStatuses = { "active", "ativo", "ativa" };
        private static readonly string[] TrialStatuses = { "trial" };
        private static readonly string[] PastDueStatuses = { "past_due", "pastdue", "inadimplente", "overdue", "atrasado" };
        private static readonly string[] CancelingStatuses = { "canceling", "cancelando", "cancelado", "canceled", "cancelled", "encerrado", "encerrada" };
        private static readonly string[] PaidStatuses = { "paid", "pago", "aprovado", "approved" };

        private static readonly string[] MonthAbbr = { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };

        public AdminOverviewRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AdminOverviewKpisDTO> GetKpis(string period)
        {
            var (curStart, curEnd, prevStart, prevEnd) = GetPeriodRange(period);
            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var thirtyDaysAgo = now.AddDays(-30);

            // Faturamento (competencia = data de pagamento) no periodo atual e anterior.
            var revenue = await _context.SubscriptionPayments.AsNoTracking()
                .Where(p => PaidStatuses.Contains(p.Status.ToLower())
                         && p.PaymentDate != null
                         && p.PaymentDate >= curStart && p.PaymentDate < curEnd)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            var revenuePrev = await _context.SubscriptionPayments.AsNoTracking()
                .Where(p => PaidStatuses.Contains(p.Status.ToLower())
                         && p.PaymentDate != null
                         && p.PaymentDate >= prevStart && p.PaymentDate < prevEnd)
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            // Novas assinaturas iniciadas no periodo atual e anterior.
            var newSubs = await _context.Subscriptions.AsNoTracking()
                .CountAsync(s => s.StartDate >= curStart && s.StartDate < curEnd);
            var newSubsPrev = await _context.Subscriptions.AsNoTracking()
                .CountAsync(s => s.StartDate >= prevStart && s.StartDate < prevEnd);

            // Canceladas no periodo (data de fim dentro da janela).
            var canceledSubs = await _context.Subscriptions.AsNoTracking()
                .CountAsync(s => s.EndDate != null && s.EndDate >= curStart && s.EndDate < curEnd);

            // Instituicoes com assinatura ativa hoje.
            var activeInstitutionsCount = await _context.Subscriptions.AsNoTracking()
                .Where(s => ActiveStatuses.Contains(s.Status.ToLower()) && (s.EndDate == null || s.EndDate > now))
                .Select(s => s.InstitutionId)
                .Distinct()
                .CountAsync();

            var trials = await _context.Subscriptions.AsNoTracking()
                .Where(s => TrialStatuses.Contains(s.Status.ToLower()) && (s.EndDate == null || s.EndDate > now))
                .Select(s => s.InstitutionId)
                .Distinct()
                .CountAsync();

            // Idosos gerenciados (visao global -- ignora o filtro de tenant).
            var totalResidents = await _context.Patients.AsNoTracking()
                .IgnoreQueryFilters()
                .CountAsync();

            var avgPerInstitution = activeInstitutionsCount > 0
                ? (int)Math.Round((double)totalResidents / activeInstitutionsCount)
                : 0;

            // Doses administradas no mes corrente + % no prazo.
            var monthDoses = await _context.MedicationAdministrations.AsNoTracking()
                .IgnoreQueryFilters()
                .Where(m => m.AdministeredDateTime != null
                         && m.AdministeredDateTime >= monthStart && m.AdministeredDateTime < monthEnd)
                .Select(m => new { m.ScheduledDateTime, m.AdministeredDateTime })
                .ToListAsync();

            var dosesValue = monthDoses.Count;
            var dosesOnTime = monthDoses.Count(m => m.AdministeredDateTime!.Value <= m.ScheduledDateTime.AddMinutes(OnTimeToleranceMinutes));
            var onTimePct = dosesValue > 0 ? Math.Round((double)dosesOnTime / dosesValue * 100, 1) : 0;

            // Adesao media por instituicao (doses no prazo / doses previstas).
            var adherenceRows = await _context.MedicationAdministrations.AsNoTracking()
                .IgnoreQueryFilters()
                .Where(m => m.Patient.InstitutionId != null)
                .Select(m => new { InstitutionId = m.Patient.InstitutionId, m.ScheduledDateTime, m.AdministeredDateTime })
                .ToListAsync();

            var adherenceByInstitution = adherenceRows
                .GroupBy(r => r.InstitutionId!.Value)
                .Select(g => AdherenceOf(g.Select(x => (x.ScheduledDateTime, x.AdministeredDateTime))))
                .ToList();

            var avgAdherence = adherenceByInstitution.Count > 0 ? Math.Round(adherenceByInstitution.Average(), 1) : 0;

            // Cuidadores ativos: funcionarios que administraram doses nos ultimos 30 dias.
            var activeCaregivers = await _context.MedicationAdministrations.AsNoTracking()
                .IgnoreQueryFilters()
                .Where(m => m.EmployeeId != null && m.AdministeredDateTime != null && m.AdministeredDateTime >= thirtyDaysAgo)
                .Select(m => m.EmployeeId)
                .Distinct()
                .CountAsync();

            return new AdminOverviewKpisDTO
            {
                Period = period,
                Revenue = new AdminRevenueKpiDTO
                {
                    Value = revenue,
                    Prev = revenuePrev,
                    ChangePct = ChangePct((double)revenue, (double)revenuePrev)
                },
                ActiveSubscriptions = new AdminActiveSubscriptionsKpiDTO
                {
                    Value = activeInstitutionsCount,
                    NetChange = newSubs - canceledSubs,
                    Trials = trials
                },
                NewSubscriptions = new AdminNewSubscriptionsKpiDTO
                {
                    Value = newSubs,
                    Prev = newSubsPrev,
                    ChangePct = ChangePct(newSubs, newSubsPrev)
                },
                ManagedResidents = new AdminManagedResidentsKpiDTO
                {
                    Value = totalResidents,
                    AvgPerInstitution = avgPerInstitution
                },
                DosesThisMonth = new AdminDosesKpiDTO
                {
                    Value = dosesValue,
                    OnTimePct = onTimePct
                },
                AvgAdherencePct = avgAdherence,
                ActiveCaregivers = activeCaregivers
            };
        }

        public async Task<List<AdminRevenueTrendPointDTO>> GetRevenueTrend(string period)
        {
            // Evolucao sempre nos ultimos 12 meses (serie historica), agregada por mes.
            var now = DateTime.Now;
            var startMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

            var payments = await _context.SubscriptionPayments.AsNoTracking()
                .Where(p => PaidStatuses.Contains(p.Status.ToLower()) && p.PaymentDate != null && p.PaymentDate >= startMonth)
                .Select(p => new { p.PaymentDate, p.Amount })
                .ToListAsync();

            var result = new List<AdminRevenueTrendPointDTO>();
            for (var i = 0; i < 12; i++)
            {
                var m = startMonth.AddMonths(i);
                var amount = payments
                    .Where(p => p.PaymentDate!.Value.Year == m.Year && p.PaymentDate.Value.Month == m.Month)
                    .Sum(p => p.Amount);

                result.Add(new AdminRevenueTrendPointDTO { Month = MonthLabel(m.Year, m.Month), Amount = amount });
            }

            return result;
        }

        public async Task<List<AdminSubscriptionMovementPointDTO>> GetSubscriptionMovement(string period)
        {
            var (curStart, curEnd, _, _) = GetPeriodRange(period);
            var startMonth = new DateTime(curStart.Year, curStart.Month, 1);
            var lastMonth = new DateTime(curEnd.Year, curEnd.Month, 1);

            var subs = await _context.Subscriptions.AsNoTracking()
                .Where(s => s.StartDate >= startMonth || (s.EndDate != null && s.EndDate >= startMonth))
                .Select(s => new { s.StartDate, s.EndDate })
                .ToListAsync();

            var result = new List<AdminSubscriptionMovementPointDTO>();
            for (var m = startMonth; m <= lastMonth; m = m.AddMonths(1))
            {
                var news = subs.Count(s => s.StartDate.Year == m.Year && s.StartDate.Month == m.Month);
                var canceled = subs.Count(s => s.EndDate != null && s.EndDate.Value.Year == m.Year && s.EndDate.Value.Month == m.Month);

                result.Add(new AdminSubscriptionMovementPointDTO { Month = MonthLabel(m.Year, m.Month), New = news, Canceled = canceled });
            }

            return result;
        }

        public async Task<List<AdminRevenueByPlanDTO>> GetRevenueByPlan(string period)
        {
            var (curStart, curEnd, _, _) = GetPeriodRange(period);
            var now = DateTime.Now;

            var revByPlan = await _context.SubscriptionPayments.AsNoTracking()
                .Where(p => PaidStatuses.Contains(p.Status.ToLower()) && p.PaymentDate != null && p.PaymentDate >= curStart && p.PaymentDate < curEnd)
                .GroupBy(p => p.Subscription.PlanId)
                .Select(g => new { PlanId = g.Key, Amount = g.Sum(x => x.Amount) })
                .ToListAsync();

            var activeSubs = await _context.Subscriptions.AsNoTracking()
                .Where(s => ActiveStatuses.Contains(s.Status.ToLower()) && (s.EndDate == null || s.EndDate > now))
                .Select(s => new { s.PlanId, s.InstitutionId })
                .ToListAsync();

            var accByPlan = activeSubs
                .GroupBy(s => s.PlanId)
                .Select(g => new { PlanId = g.Key, Accounts = g.Select(x => x.InstitutionId).Distinct().Count() })
                .ToList();

            var plans = await _context.Plans.AsNoTracking()
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            var total = revByPlan.Sum(r => r.Amount);

            return plans
                .Select(pl =>
                {
                    var amount = revByPlan.FirstOrDefault(r => r.PlanId == pl.Id)?.Amount ?? 0m;
                    var accounts = accByPlan.FirstOrDefault(a => a.PlanId == pl.Id)?.Accounts ?? 0;

                    return new AdminRevenueByPlanDTO
                    {
                        Plan = pl.Name,
                        Amount = amount,
                        Accounts = accounts,
                        Percentage = total > 0 ? Math.Round((double)amount / (double)total * 100, 1) : 0
                    };
                })
                .Where(x => x.Amount > 0 || x.Accounts > 0)
                .OrderByDescending(x => x.Amount)
                .ToList();
        }

        public async Task<List<AdminSubscriptionStatusDTO>> GetSubscriptionStatus()
        {
            var statuses = await _context.Subscriptions.AsNoTracking()
                .Select(s => s.Status)
                .ToListAsync();

            return statuses
                .GroupBy(NormalizeStatus)
                .Select(g => new AdminSubscriptionStatusDTO { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        public async Task<AdminInstitutionsPageDTO> GetInstitutions(string? search, string? status, long? planId, string? sort, int page, int perPage)
        {
            // As instituicoes sao os tenants do SaaS (poucos registros por natureza):
            // carregamos os agregados por instituicao e paginamos em memoria.
            var institutions = await _context.Institutions.AsNoTracking()
                .Select(i => new { i.Id, i.Name })
                .ToListAsync();

            var subs = await _context.Subscriptions.AsNoTracking()
                .Select(s => new
                {
                    s.InstitutionId,
                    s.PlanId,
                    s.Status,
                    s.StartDate,
                    s.EndDate,
                    PlanName = s.Plan.Name,
                    PlanPrice = s.Plan.MonthlyPrice
                })
                .ToListAsync();

            var residentCounts = await _context.Patients.AsNoTracking()
                .IgnoreQueryFilters()
                .Where(p => p.InstitutionId != null)
                .GroupBy(p => p.InstitutionId)
                .Select(g => new { InstitutionId = g.Key, Count = g.Count() })
                .ToListAsync();
            var residentDict = residentCounts.ToDictionary(r => r.InstitutionId!.Value, r => r.Count);

            var adherenceRows = await _context.MedicationAdministrations.AsNoTracking()
                .IgnoreQueryFilters()
                .Where(m => m.Patient.InstitutionId != null)
                .Select(m => new { InstitutionId = m.Patient.InstitutionId, m.ScheduledDateTime, m.AdministeredDateTime })
                .ToListAsync();
            var adherenceDict = adherenceRows
                .GroupBy(r => r.InstitutionId!.Value)
                .ToDictionary(g => g.Key, g => AdherenceOf(g.Select(x => (x.ScheduledDateTime, x.AdministeredDateTime))));

            var rows = institutions.Select(i =>
            {
                var current = subs
                    .Where(s => s.InstitutionId == i.Id)
                    .OrderByDescending(s => s.StartDate)
                    .FirstOrDefault();

                var plan = current != null
                    ? new AdminPlanOptionDTO { Id = current.PlanId, Name = current.PlanName, MonthlyPrice = current.PlanPrice }
                    : null;

                return new AdminInstitutionRowDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    PlanId = current?.PlanId,
                    Plan = plan,
                    Status = current != null ? NormalizeStatus(current.Status) : string.Empty,
                    MonthlyRevenue = current?.PlanPrice ?? 0m,
                    Residents = residentDict.TryGetValue(i.Id, out var rc) ? rc : 0,
                    AdherencePct = adherenceDict.TryGetValue(i.Id, out var ad) ? ad : 0,
                    RenewsAt = current?.EndDate?.ToString("yyyy-MM-dd")
                };
            }).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                rows = rows.Where(r => r.Name.ToLowerInvariant().Contains(term)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                var st = status.Trim().ToLowerInvariant();
                rows = rows.Where(r => r.Status == st).ToList();
            }
            if (planId.HasValue)
            {
                rows = rows.Where(r => r.PlanId == planId.Value).ToList();
            }

            rows = ApplySort(rows, sort);

            var count = rows.Count;
            if (perPage <= 0) perPage = 10;
            var totalPages = Math.Max((int)Math.Ceiling((double)count / perPage), 1);
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = rows.Skip((page - 1) * perPage).Take(perPage).ToList();

            return new AdminInstitutionsPageDTO
            {
                Items = items,
                Page = page,
                PerPage = perPage,
                TotalPages = totalPages,
                Count = count
            };
        }

        public async Task<List<AdminPlanOptionDTO>> GetPlans()
        {
            return await _context.Plans.AsNoTracking()
                .Where(p => p.Active)
                .OrderBy(p => p.MonthlyPrice)
                .Select(p => new AdminPlanOptionDTO { Id = p.Id, Name = p.Name, MonthlyPrice = p.MonthlyPrice })
                .ToListAsync();
        }

        // ---------- Helpers ----------

        private static double AdherenceOf(IEnumerable<(DateTime Scheduled, DateTime? Administered)> doses)
        {
            var list = doses.ToList();
            if (list.Count == 0) return 0;
            var onTime = list.Count(d => d.Administered != null && d.Administered.Value <= d.Scheduled.AddMinutes(OnTimeToleranceMinutes));
            return Math.Round((double)onTime / list.Count * 100, 1);
        }

        private static List<AdminInstitutionRowDTO> ApplySort(List<AdminInstitutionRowDTO> rows, string? sort)
        {
            return sort switch
            {
                "revenue_asc" => rows.OrderBy(r => r.MonthlyRevenue).ToList(),
                "name_asc" => rows.OrderBy(r => r.Name).ToList(),
                "name_desc" => rows.OrderByDescending(r => r.Name).ToList(),
                "residents_desc" => rows.OrderByDescending(r => r.Residents).ToList(),
                "adherence_desc" => rows.OrderByDescending(r => r.AdherencePct).ToList(),
                _ => rows.OrderByDescending(r => r.MonthlyRevenue).ToList(),
            };
        }

        private static string NormalizeStatus(string? status)
        {
            var s = (status ?? string.Empty).Trim().ToLowerInvariant();
            if (ActiveStatuses.Contains(s)) return "active";
            if (TrialStatuses.Contains(s)) return "trial";
            if (PastDueStatuses.Contains(s)) return "past_due";
            if (CancelingStatuses.Contains(s)) return "canceling";
            return string.IsNullOrEmpty(s) ? "unknown" : s;
        }

        private static double? ChangePct(double current, double previous)
        {
            if (previous == 0) return null;
            return Math.Round((current - previous) / previous * 100, 1);
        }

        private static (DateTime curStart, DateTime curEnd, DateTime prevStart, DateTime prevEnd) GetPeriodRange(string period)
        {
            var now = DateTime.Now;
            DateTime curStart;
            switch (period)
            {
                case "30d":
                    curStart = now.AddDays(-30);
                    break;
                case "12m":
                    curStart = now.AddMonths(-12);
                    break;
                case "90d":
                default:
                    curStart = now.AddDays(-90);
                    break;
            }

            var span = now - curStart;
            var prevEnd = curStart;
            var prevStart = curStart - span;
            return (curStart, now, prevStart, prevEnd);
        }

        private static string MonthLabel(int year, int month)
        {
            return $"{MonthAbbr[month - 1]}/{year % 100:D2}";
        }
    }
}
