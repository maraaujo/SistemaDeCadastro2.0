using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;

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

        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
        {
            var ret =
                from pa in _context.Patients
                join pcc in _context.PatientClinicalConditions
                    on pa.Id equals pcc.PatientId
                join cc in _context.ClinicalConditions
                    on pcc.ClinicalConditionId equals cc.Id
                join mpcc in _context.MedicinePatientClinicalConditions
                    on pcc.Id equals mpcc.PatientClinicalConditionId
                join med in _context.Medicines
                    on mpcc.MedicineId equals med.Id
                join resp in _context.Responsibles
                    on pa.Id equals resp.PatientId into respGroup
                from resp in respGroup.DefaultIfEmpty()
                select new
                {
                    Patient = pa,
                    ClinicalCondition = cc,
                    MedicinePatientClinicalCondition = mpcc,
                    Medicine = med,
                    Responsible = resp
                };

            if (!string.IsNullOrEmpty(filter.Name))
            {
                ret = ret.Where(c => c.Patient.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Illness))
            {
                ret = ret.Where(c => c.ClinicalCondition.Name.Contains(filter.Illness));
            }

            if (!string.IsNullOrEmpty(filter.Medicine))
            {
                ret = ret.Where(c => c.Medicine.Name.Contains(filter.Medicine));
            }

            if (!string.IsNullOrEmpty(filter.Dosage))
            {
                ret = ret.Where(c => c.MedicinePatientClinicalCondition.PrescribedDosage.Contains(filter.Dosage));
            }

            if (!string.IsNullOrEmpty(filter.Responsible))
            {
                ret = ret.Where(c =>
                    c.Responsible != null &&
                    c.Responsible.Name.Contains(filter.Responsible));
            }

            var result = await ret.Select(c => new PatientFilterDTO
            {
                Name = c.Patient.Name,
                Illness = c.ClinicalCondition.Name,
                Medicine = c.Medicine.Name,
                Dosage = c.MedicinePatientClinicalCondition.PrescribedDosage,
                Responsible = c.Responsible != null ? c.Responsible.Name : null
            }).ToListAsync();

            return result;
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