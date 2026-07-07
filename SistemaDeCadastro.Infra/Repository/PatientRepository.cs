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
                        Dosage = mpcc.PrescribedDosage,

                        // No DER novo não temos mais Time nem histórico LastTime.
                        // Mantive sem preencher para evitar erro de compilação.
                        // Se quiser, podemos depois trocar Time por Frequency.
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
                Responsible = c.Responsible != null ? c.Responsible.Name : null,

                // No DER novo não existe mais Time.
                // Se o DTO ainda tiver Time, ele ficará com valor padrão 0.
            }).ToListAsync();

            return result;
        }

        public async Task<List<Patient>> GetPatientById(long id)
        {
            return await FindBy(c => c.Id == id);
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

        public async Task<List<MedicinePatientIllnessDTO>> GetMedicinesToMinister()
        {
            var data = await _context.MedicinePatientClinicalConditions
                .Select(c => new MedicinePatientIllnessDTO
                {
                    Patient = c.PatientClinicalCondition.Patient.Name,
                    Medicine = c.Medicine.Name,
                    Illness = c.PatientClinicalCondition.ClinicalCondition.Name,

                    // No DER novo, Time foi substituído por Frequency.
                    // Como o DTO antigo ainda usa Time, deixei sem preencher.
                    // Podemos depois ajustar o DTO para Frequency.

                    MedicineHistoric = new List<MedicinePatientHistoricDTO>()
                })
                .ToListAsync();

            return data;
        }
    }
}