using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public PatientRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<DetailsPatientDTO>> DetailsPatient()
        {
            try
            {
                
                var result = await (
                    from p in _context.Patients
                    join pcc in _context.PatientClinicalConditions
                        on p.Id equals pcc.PatientId
                    join cc in _context.ClinicalConditions
                        on pcc.ClinicalConditionId equals cc.Id
                    join mpcc in _context.MedicinePatientClinicalConditions
                        on pcc.Id equals mpcc.PatientClinicalConditionId
                    join med in _context.Medicines
                        on mpcc.MedicineId equals med.Id
                    select new DetailsPatientDTO
                    {
                        Name = p.Name,
                        IllnessName = cc.Name,
                        MedicineName = med.Name,
                        Dosage = mpcc.PrescribedDosage
                    }
                ).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar detalhes do paciente. Detalhes: " + ex.Message, ex);
            }
        }

        
        public async Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;

            var query =
                from pa in _context.Patients.AsNoTracking()

                join re in _context.Responsibles.AsNoTracking()
                    on pa.Id equals re.PatientId into responsibleGroup
                from re in responsibleGroup.DefaultIfEmpty()

                join pcc in _context.PatientClinicalConditions.AsNoTracking()
                    on pa.Id equals pcc.PatientId into pccGroup
                from pcc in pccGroup.DefaultIfEmpty()

                join cc in _context.ClinicalConditions.AsNoTracking()
                    on pcc.ClinicalConditionId equals cc.Id into ccGroup
                from cc in ccGroup.DefaultIfEmpty()

                join mpcc in _context.MedicinePatientClinicalConditions.AsNoTracking()
                    on pcc.Id equals mpcc.PatientClinicalConditionId into mpccGroup
                from mpcc in mpccGroup.DefaultIfEmpty()

                join med in _context.Medicines.AsNoTracking()
                    on mpcc.MedicineId equals med.Id into medGroup
                from med in medGroup.DefaultIfEmpty()

                select new PatientListDTO
                {
                    Id = pa.Id,
                    Name = pa.Name,

                    ResponsibleName = re != null ? re.Name : null,

                    ClinicalCondition = cc != null ? cc.Name : null,

                    Medicine = med != null ? med.Name : null,

                    Dosage = mpcc != null ? mpcc.PrescribedDosage : null,

                    Time = mpcc != null ? mpcc.AdministrationTime : null
                };

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(c => c.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrWhiteSpace(filter.ResponsibleName))
            {
                query = query.Where(c => c.ResponsibleName != null &&
                                         c.ResponsibleName.Contains(filter.ResponsibleName));
            }

            if (!string.IsNullOrWhiteSpace(filter.ClinicalCondition))
            {
                query = query.Where(c => c.ClinicalCondition != null &&
                                         c.ClinicalCondition.Contains(filter.ClinicalCondition));
            }

            if (!string.IsNullOrWhiteSpace(filter.Medicine))
            {
                query = query.Where(c => c.Medicine != null &&
                                         c.Medicine.Contains(filter.Medicine));
            }

            if (!string.IsNullOrWhiteSpace(filter.Dosage))
            {
                query = query.Where(c => c.Dosage != null &&
                                         c.Dosage.Contains(filter.Dosage));
            }

            var ret = new PagedPatientDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Patients = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
        

        public async Task<List<Patient>> GetPatientById(long id)
        {
            return await FindBy(c => c.Id == id);
        }

        public async Task<Patient?> GetByIdWithRelations(long id)
        {
            return await _context.Patients
                .Include(p => p.BloodType)
                .Include(p => p.Responsibles)
                .Include(p => p.PatientClinicalConditions)
                    .ThenInclude(pcc => pcc.ClinicalCondition)
                .Include(p => p.PatientIllnesses)
                    .ThenInclude(pi => pi.Illness)
                .Include(p => p.PatientEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Appointments)
                .Include(p => p.CareServices)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        //vou ter que fazer um job para ficar chamando esse metodo de tempos
        //em tempos para ficar atualizando a tela de lembrete de medicamentos
        public async Task<List<MedicineReminderDTO>> GetMedicineReminders()
        {
            var result = new List<MedicineReminderDTO>();

            var now = DateTime.Now;
            var today = now.Date;

            var connection = _context.Database.GetDbConnection();
            var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

            if (shouldCloseConnection)
                await connection.OpenAsync();

            try
            {
                using var command = connection.CreateCommand();

                command.CommandText = @"
            SELECT 
                p.id_acolhido AS PatientId,
                p.nome AS PatientName,
                med.nome AS MedicineName,
                mpcc.dosagem_prescrita AS Dosage,
                mpcc.frequencia AS Frequency,
                mpcc.horario_administracao AS AdministrationTime,
                f.nome AS ResponsibleEmployeeName
            FROM medicamento_acolhido_condicaoclinica mpcc
            INNER JOIN medicamentos med 
                ON med.id_medicamento = mpcc.id_medicamento
            INNER JOIN acolhido_condicaoclinica pcc 
                ON pcc.id_acolhido_condicao = mpcc.id_acolhido_condicao
            INNER JOIN acolhidos p 
                ON p.id_acolhido = pcc.id_acolhido
            LEFT JOIN funcionarios f 
                ON f.id_funcionario = mpcc.id_funcionario_responsavel
            WHERE mpcc.horario_administracao IS NOT NULL
              AND (mpcc.data_fim IS NULL OR mpcc.data_fim >= CURDATE())
        ";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var administrationTime = reader["AdministrationTime"] == DBNull.Value
                        ? (TimeSpan?)null
                        : (TimeSpan)reader["AdministrationTime"];

                    if (administrationTime == null)
                        continue;

                    var nextDoseDateTime = today.Add(administrationTime.Value);

                    if (nextDoseDateTime < now)
                        nextDoseDateTime = nextDoseDateTime.AddDays(1);

                    var minutesRemaining = (int)(nextDoseDateTime - now).TotalMinutes;

                    string alertText;

                    if (minutesRemaining <= 5)
                        alertText = "Faltam 5 minutos ou menos";
                    else if (minutesRemaining <= 15)
                        alertText = "Faltam 15 minutos ou menos";
                    else if (minutesRemaining <= 30)
                        alertText = "Faltam 30 minutos ou menos";
                    else if (minutesRemaining <= 60)
                        alertText = "Falta 1 hora ou menos";
                    else
                        alertText = $"Faltam {minutesRemaining} minutos";

                    result.Add(new MedicineReminderDTO
                    {
                        PatientId = Convert.ToInt64(reader["PatientId"]),
                        PatientName = reader["PatientName"]?.ToString(),
                        MedicineName = reader["MedicineName"]?.ToString(),
                        Dosage = reader["Dosage"]?.ToString(),
                        Frequency = reader["Frequency"]?.ToString(),
                        AdministrationTime = administrationTime,
                        NextDoseDateTime = nextDoseDateTime,
                        ResponsibleEmployeeName = reader["ResponsibleEmployeeName"] == DBNull.Value
                            ? "Não informado"
                            : reader["ResponsibleEmployeeName"]?.ToString(),
                        MinutesRemaining = minutesRemaining,
                        AlertText = alertText
                    });
                }
            }
            finally
            {
                if (shouldCloseConnection)
                    await connection.CloseAsync();
            }

            return result
                .OrderBy(x => x.MinutesRemaining)
                .ToList();
        }

        public async Task CreatePatient(Patient patient)
        {
            await Create(patient);
        }

        public async Task UpdatePatient(Patient patient)
        {
            await Update(patient);
        }

        public async Task DeletePatient(Patient patient)
        {
            await Delete(patient);
        }

        public async Task GetPatientByAny(string patient)
        {
            await Any(c => c.Name == patient);
        }
    }
}