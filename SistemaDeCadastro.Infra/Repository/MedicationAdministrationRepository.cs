using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicationAdministrationRepository : BaseRepository<MedicationAdministration>, IMedicationAdministrationRepository
    {
        private readonly SistemaDeCadastroContext _context;
        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public MedicationAdministrationRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }

        /// <summary>Busca a administracao de medicamento pelo id; devolve lista com 0 ou 1 item (somente da instituicao logada).</summary>
        public async Task<List<MedicationAdministration>> GetById(long ind)
        {
            return await this.FindBy(c => c.Id == ind);
        }

        // Apaga todas as administracoes de todas as instituicoes (ignora o filtro de
        // tenant). Retorna a quantidade de linhas removidas.
        public async Task<int> DeleteAllGlobal()
        {
            return await _context.MedicationAdministrations
                .IgnoreQueryFilters()
                .ExecuteDeleteAsync();
        }
        /// <summary>Lista as administracoes de medicamento cujo Status e igual ao texto informado.</summary>
        public async Task<List<MedicationAdministration>> GetMedicationAdministrationByStatus(string status)
        {
            return await this.FindBy(c => c.Status == status);
        }

        // Delete vem do BaseRepository. O metodo que existia aqui chamava a si mesmo
        // (this.Delete) e o StackOverflow derrubava o processo inteiro da API.

        /// <summary>
        /// Historico paginado de administracoes de medicamento (tela de historico de medicacao).
        /// Junta MedicationAdministrations com Patients (inner join) e faz left join com Employees (registros
        /// sem funcionario tambem aparecem), com o medicamento programado e com o medicamento. Filtros opcionais:
        /// id, status exato, acolhido, funcionario, medicamento, horario previsto e horario administrado
        /// (janela de 1 minuto) e observacoes (texto exato). Ordena do maior id para o menor, 15 por pagina.
        /// Depois, em memoria, calcula a "proxima medicacao" (hoje + horario planejado, fuso de Sao Paulo) e
        /// corrige horario previsto 00:00 de registros antigos usando o horario planejado.
        /// </summary>
        public async Task<PagedMedicationAdministrationDTO> GetMedicationAdministrationByFilter(MedicationAdministrationFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page; 
            var medicineId = filter.MedicineId;

            var query = from m in _context.MedicationAdministrations
                        join p in _context.Patients on m.PatientId equals p.Id
                        // Funcionario e opcional no registro: LEFT JOIN para a administracao
                        // sem funcionario nao sumir do historico.
                        join e in _context.Employees on m.EmployeeId equals (long?)e.Id into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        join mpcc in _context.MedicinePatientClinicalConditions
                            on m.MedicinePatientClinicalConditionId equals mpcc.Id into mpccGroup
                        from mpcc in mpccGroup.DefaultIfEmpty()
                        join med in _context.Medicines
                            on mpcc.MedicineId equals med.Id into medGroup
                        from med in medGroup.DefaultIfEmpty()
                        where medicineId == null || (mpcc != null && mpcc.MedicineId == medicineId)
                        select new MedicationAdministrationDTO
                        {
                            Id = m.Id,
                            MedicinePatientClinicalConditionId = m.MedicinePatientClinicalConditionId,
                            PatientId = m.PatientId,
                            NamePatient = p.Name,
                            MedicineName = med != null ? med.Name : null,
                            PrescribedDosage = mpcc != null ? mpcc.PrescribedDosage : null,
                            EmployeeId = m.EmployeeId,
                            EmployeeName = e != null ? e.Name : null,
                            ScheduledDateTime = m.ScheduledDateTime,
                            AdministeredDateTime = m.AdministeredDateTime,
                            Status = m.Status,
                            Observations = m.Observations,
                            CreatedAt = m.CreatedAt,
                            // Horario planejado do medicamento programado (para exibir o
                            // "horario previsto" e a "proxima medicacao" na tela de historico).
                            AdministrationTime = mpcc != null ? mpcc.AdministrationTime : null
                        };
            if(filter.Id.HasValue)
            {
                query = query.Where(c => c.Id == filter.Id.Value);
            }
            if(!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(c => c.Status == filter.Status);
            }
            if(filter.PatientId.HasValue)
            {
                query = query.Where(c => c.PatientId == filter.PatientId.Value);
            }
            if(filter.EmployeeId.HasValue) {
                query = query.Where(c => c.EmployeeId == filter.EmployeeId.Value);
            }
            if (filter.ScheduledDateTime.HasValue)
            {

                var startTime = filter.ScheduledDateTime.Value;
                var endTime = startTime.Add(TimeSpan.FromMinutes(1));

                query = query.Where(a => a.ScheduledDateTime >= startTime && a.ScheduledDateTime < endTime);
            }
            if (filter.AdministeredDateTime.HasValue)
            {
                var startTime = filter.AdministeredDateTime.Value;
                var endTime = startTime.Add(TimeSpan.FromMinutes(1));
                query = query.Where(a => a.AdministeredDateTime >= startTime && a.AdministeredDateTime < endTime);
            }
            if (!string.IsNullOrEmpty(filter.Observations))
            {
                query = query.Where(c => c.Observations == filter.Observations);    
            }
            var ret = new PagedMedicationAdministrationDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.MedicationAdministration = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            // Preenche a "proxima medicacao" (hoje + horario planejado) com fuso explicito,
            // e, para registros antigos cujo horario previsto veio zerado (00:00), usa o
            // horario planejado do medicamento programado como horario previsto.
            var today = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo().Date;
            foreach (var dto in ret.MedicationAdministration)
            {
                if (dto.AdministrationTime.HasValue)
                {
                    dto.NextDoseDateTime = SistemaDeCadastro.Domain.Utils.DateTimeUtils
                        .ToSaoPauloOffset(today.Add(dto.AdministrationTime.Value));

                    if (dto.ScheduledDateTime.TimeOfDay == TimeSpan.Zero)
                    {
                        dto.ScheduledDateTime = dto.ScheduledDateTime.Date.Add(dto.AdministrationTime.Value);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Monta os lembretes da tela "Proximos medicamentos" para hoje (fuso de Sao Paulo).
        /// 1) Carrega as administracoes ja registradas hoje. 2) Busca os medicamentos programados com
        /// horario definido e vigentes hoje (StartDate/EndDate), juntando medicamento, condicao do acolhido,
        /// acolhido e responsavel (left join). 3) Em memoria, descarta os que ja tem registro hoje, seja dose
        /// dada ou marcada como "Nao administrado"/"Cancelado"
        /// (mesmo medicamento programado + mesmo acolhido) e calcula minutos restantes/atraso e o texto
        /// do alerta. Ordena pelo menor tempo restante (atrasados primeiro). Restrito a instituicao logada.
        /// </summary>
        public async Task<List<MedicineReminderDTO>> GetMedicineReminders()
        {
            // O servidor pode rodar em UTC (ex.: Cloud Run). Calcula "agora" no fuso de
            // Sao Paulo para que o horario previsto e o atraso batam com o horario local.
            // Mantido como Unspecified (sem 'Z'/offset) para o front exibir a hora de parede.
            var now = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo();
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            /*
                Busca as administrações registradas hoje.

                Aqui usamos tanto ScheduledDateTime quanto AdministeredDateTime porque,
                em alguns registros antigos, o ScheduledDateTime pode ter sido salvo como
                00:00:00, mas o AdministeredDateTime está correto.

                O filtro usa intervalo (>= hoje e < amanhã) em vez de ".Date == hoje":
                assim o banco consegue usar índice nas colunas de data, sem varrer a
                tabela inteira aplicando uma função em cada linha.
            */
            var administrationsToday = await _context.MedicationAdministrations
                .AsNoTracking()
                .Where(x =>
                    (x.ScheduledDateTime >= today && x.ScheduledDateTime < tomorrow) ||
                    (x.AdministeredDateTime != null &&
                     x.AdministeredDateTime >= today && x.AdministeredDateTime < tomorrow)
                )
                .Select(x => new { x.MedicinePatientClinicalConditionId, x.PatientId })
                .ToListAsync();

            /*
                Busca todos os medicamentos programados para hoje.

                Importante:
                - StartDate define quando o medicamento começa a aparecer.
                - EndDate define quando ele para de aparecer.
                - Se EndDate for null, é considerado uso contínuo.
            */
            var query =
                from mpcc in _context.MedicinePatientClinicalConditions.AsNoTracking()

                join medicine in _context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id

                join pcc in _context.PatientClinicalConditions.AsNoTracking()
                    on mpcc.PatientClinicalConditionId equals pcc.Id

                join patient in _context.Patients.AsNoTracking()
                    on pcc.PatientId equals patient.Id

                join employee in _context.Employees.AsNoTracking()
                    on mpcc.ResponsibleEmployeeId equals (long?)employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()

                where mpcc.AdministrationTime != null
                      && (mpcc.StartDate == null || mpcc.StartDate < tomorrow)
                      && (mpcc.EndDate == null || mpcc.EndDate >= today)

                select new
                {
                    MedicinePatientClinicalConditionId = mpcc.Id,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    Dosage = mpcc.PrescribedDosage,
                    Frequency = mpcc.Frequency,
                    AdministrationTime = mpcc.AdministrationTime,
                    StartDate = mpcc.StartDate,
                    EndDate = mpcc.EndDate,
                    ResponsibleEmployeeName = employee != null ? employee.Name : "Não informado"
                };

            var items = await query.ToListAsync();

                        var result = items
                            .Select(item =>
                            {
                                /*
                                    Monta a data/hora prevista real.

                                    Exemplo:
                                    today = 2026-08-24 00:00:00
                                    AdministrationTime = 10:45:00

                                    Resultado:
                                    nextDoseDateTime = 2026-08-24 10:45:00
                                */
                                var nextDoseDateTime = today.Add(item.AdministrationTime.Value);

                                /*
                                    Verifica se a dose de hoje deste medicamento programado já foi
                                    tratada, ou seja, se já existe um registro para ela hoje.

                                    item = medicamento programado que aparece em "Próximos Medicamentos"
                                    a = registro já salvo em "Administração de Medicamentos"

                                    Não vamos comparar o horário exato aqui, porque seus registros antigos
                                    salvaram ScheduledDateTime como 00:00:00.

                                    Vale qualquer registro do dia: dose administrada OU registrada
                                    como "Não administrado"/"Cancelado". Nos dois casos a equipe já
                                    decidiu o que fazer com a dose, então o lembrete sai da lista.
                                    Quem diferencia "dada" de "não dada" é o relatório do dia.
                                */
                                var alreadyAdministered = administrationsToday.Any(a =>
                                     a.MedicinePatientClinicalConditionId == item.MedicinePatientClinicalConditionId &&
                                     a.PatientId == item.PatientId);

                                /*
                                    Se a dose de hoje já foi registrada, não deve aparecer mais
                                    na tela de Próximos Medicamentos.
                                */
                                if (alreadyAdministered)
                        return null;

                    // Arredonda SEMPRE para cima (em modulo), para a contagem nunca mostrar
                    // menos tempo do que realmente falta (evita a sensacao de "comer um
                    // minuto" ao truncar os segundos). Ex.: faltando 1min50s -> "Em 2 min".
                    var totalMinutes = (nextDoseDateTime - now).TotalMinutes;
                    var minutesRemaining = totalMinutes >= 0
                        ? (int)Math.Ceiling(totalMinutes)
                        : -(int)Math.Ceiling(-totalMinutes);

                    var alertText = BuildAlertText(minutesRemaining);

                    return new MedicineReminderDTO
                    {
                        MedicinePatientClinicalConditionId = item.MedicinePatientClinicalConditionId,
                        PatientId = item.PatientId,
                        PatientName = item.PatientName,
                        MedicineName = item.MedicineName,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        AdministrationTime = item.AdministrationTime,
                        // Envia com fuso explicito (-03:00) para o front calcular o instante certo.
                        NextDoseDateTime = SistemaDeCadastro.Domain.Utils.DateTimeUtils.ToSaoPauloOffset(nextDoseDateTime),
                        MinutesRemaining = minutesRemaining,
                        AlertText = alertText,
                        ResponsibleEmployeeName = item.ResponsibleEmployeeName
                    };
                })
                .Where(x => x != null)
                .OrderBy(x => x.MinutesRemaining)
                .ToList();

            return result;
        }

        // Monta o texto de alerta em horas/minutos (ex.: "Em 2h 05min", "Atrasado há 1h 10min").
        private static string BuildAlertText(int minutesRemaining)
        {
            if (minutesRemaining == 0)
                return "Administrar agora";

            var atrasado = minutesRemaining < 0;
            var total = Math.Abs(minutesRemaining);
            var horas = total / 60;
            var minutos = total % 60;

            string tempo;
            if (horas > 0 && minutos > 0)
                tempo = $"{horas}h {minutos:D2}min";
            else if (horas > 0)
                tempo = $"{horas}h";
            else
                tempo = $"{minutos} min";

            return atrasado ? $"Atrasado há {tempo}" : $"Em {tempo}";
        }

        // Relatorio de medicamentos do dia: todos os medicamentos programados para a data de
        // referencia (hoje, por padrao) com o status calculado (Administrado / Nao administrado /
        // Cancelado / Atrasado / Pendente). Opcionalmente filtra por um acolhido (paciente).
        public async Task<List<DailyMedicationReportItemDTO>> GetDailyMedicationReport(DateTime? referenceDate, long? patientId)
        {
            var now = SistemaDeCadastro.Domain.Utils.DateTimeUtils.NowSaoPaulo();
            var day = (referenceDate ?? now).Date;
            var nextDay = day.AddDays(1);

            // Intervalo (>= dia e < dia seguinte) em vez de ".Date == dia", para o
            // banco poder usar indice nas colunas de data.
            var administrationsOnDay = await _context.MedicationAdministrations
                .AsNoTracking()
                .Where(x =>
                    (x.ScheduledDateTime >= day && x.ScheduledDateTime < nextDay) ||
                    (x.AdministeredDateTime != null &&
                     x.AdministeredDateTime >= day && x.AdministeredDateTime < nextDay))
                .ToListAsync();

            var query =
                from mpcc in _context.MedicinePatientClinicalConditions.AsNoTracking()
                join medicine in _context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals medicine.Id
                join pcc in _context.PatientClinicalConditions.AsNoTracking()
                    on mpcc.PatientClinicalConditionId equals pcc.Id
                join patient in _context.Patients.AsNoTracking()
                    on pcc.PatientId equals patient.Id
                join employee in _context.Employees.AsNoTracking()
                    on mpcc.ResponsibleEmployeeId equals (long?)employee.Id into employeeGroup
                from employee in employeeGroup.DefaultIfEmpty()
                where mpcc.AdministrationTime != null
                      && (mpcc.StartDate == null || mpcc.StartDate < nextDay)
                      && (mpcc.EndDate == null || mpcc.EndDate >= day)
                      && (patientId == null || patient.Id == patientId.Value)
                select new
                {
                    MedicinePatientClinicalConditionId = mpcc.Id,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    MedicineName = medicine.Name,
                    Dosage = mpcc.PrescribedDosage,
                    Frequency = mpcc.Frequency,
                    AdministrationTime = mpcc.AdministrationTime,
                    ResponsibleEmployeeName = employee != null ? employee.Name : "Não informado"
                };

            var items = await query.ToListAsync();

            var result = items
                .Select(item =>
                {
                    var scheduled = day.Add(item.AdministrationTime.Value);

                    var records = administrationsOnDay
                        .Where(a =>
                            a.MedicinePatientClinicalConditionId == item.MedicinePatientClinicalConditionId &&
                            a.PatientId == item.PatientId)
                        .ToList();

                    // Dose efetivamente dada (o status decide; registro "Não administrado"
                    // ou "Cancelado" nunca conta como dada, mesmo que tenha horario gravado).
                    var administration = records.FirstOrDefault(a =>
                        SistemaDeCadastro.Domain.Utils.MedicationStatus.WasGiven(a.Status, a.AdministeredDateTime));

                    // Dose registrada como nao dada (recusa, cancelamento...).
                    var notGiven = administration == null
                        ? records.FirstOrDefault(a =>
                            SistemaDeCadastro.Domain.Utils.MedicationStatus.IsNotAdministered(a.Status))
                        : null;

                    string status;
                    DateTimeOffset? administeredAt = null;
                    string observations = string.Empty;

                    if (administration != null)
                    {
                        status = "Administrado";
                        observations = administration.Observations ?? string.Empty;
                        if (administration.AdministeredDateTime != null)
                            administeredAt = SistemaDeCadastro.Domain.Utils.DateTimeUtils
                                .ToSaoPauloOffset(administration.AdministeredDateTime.Value);
                    }
                    else if (notGiven != null)
                    {
                        status = notGiven.Status!.Trim().StartsWith("Cancel", StringComparison.OrdinalIgnoreCase)
                            ? "Cancelado"
                            : "Não administrado";
                        observations = notGiven.Observations ?? string.Empty;
                    }
                    else
                    {
                        status = scheduled < now ? "Atrasado" : "Pendente";
                    }

                    return new DailyMedicationReportItemDTO
                    {
                        MedicinePatientClinicalConditionId = item.MedicinePatientClinicalConditionId,
                        PatientId = item.PatientId,
                        PatientName = item.PatientName,
                        MedicineName = item.MedicineName,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        ResponsibleEmployeeName = item.ResponsibleEmployeeName,
                        AdministrationTime = item.AdministrationTime,
                        ScheduledDateTime = SistemaDeCadastro.Domain.Utils.DateTimeUtils.ToSaoPauloOffset(scheduled),
                        AdministeredDateTime = administeredAt,
                        Status = status,
                        Observations = observations
                    };
                })
                .OrderBy(x => x.PatientName)
                .ThenBy(x => x.ScheduledDateTime)
                .ToList();

            return result;
        }
    }
}
