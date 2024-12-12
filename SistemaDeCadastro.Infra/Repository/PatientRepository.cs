using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        private readonly IMedicinePatientIllnessRepository _medicinePatientIllnessRepository;
        private readonly SistemaCadastroContext _context;
        public PatientRepository(SistemaCadastroContext context 
           )
            : base(context)
        {
        }
        public async Task<List<DetailsPatientDTO>> DetailsPatient()
        {


            try
            {

                var ret = (from p in _context.Patients
                           join mpi in _context.MedicinePatientIllnesses
                           on p.Id equals mpi.IdPatient
                           join ill in _context.Illnesses on mpi.IdIllness equals ill.Id
                           join me in _context.Medicines on mpi.IdMedicine equals me.Id
                           join mpih in _context.MedicinePatientIllnessHistorics on mpi.Id equals mpih.IdMedicinePatientIllness
              
                           select new
                           {
                               p,
                               ill,
                               me,
                               mpi,
                               mpih
                           });
                var result = await ret.Select(c => new DetailsPatientDTO
                {
                    Name = c.p.Name,
                    IllnessName = c.ill.Name,
                    MedicineName = c.me.Name,
                    Dosage = c.mpi.Dosage,
                    Time = c.mpi.Time,
                    LastTime = c.mpih.LastTime,

                }).ToListAsync();

                return result;
            }
            catch (Exception err)
            {
                throw new Exception("Erro ao buscar detalhes do paciente. Detalhes: \" + ex.Message, ex");
            }


        }

        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
        {
            var ret = (from pa in _context.Patients
                          join mePa in _context.MedicinePatientIllnesses
                              on pa.Id equals mePa.IdPatient
                          join ill in _context.Illnesses on mePa.IdIllness equals ill.Id
                          join me in _context.Medicines
                              on mePa.IdMedicine equals me.Id
                          select new
                          {
                                    pa,
                                    ill,
                                    mePa,
                                    me,
                                });
            
            if (!string.IsNullOrEmpty(filter.Name))
            {
                ret = ret.Where(c => c.pa.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.Illness))
            {
                ret = ret.Where(c => c.ill.Name.Contains(filter.Illness));
            }
            if (!string.IsNullOrEmpty(filter.Medicine))
            {
                ret = ret.Where(c => c.me.Name.Contains(filter.Medicine));
            }
            if (!string.IsNullOrEmpty(filter.Dosage))
            {
                ret = ret.Where(c => c.mePa.Dosage.Contains(filter.Dosage));
            }
            if (!string.IsNullOrEmpty(filter.Responsible))
            {
                ret = ret.Where(c => c.pa.Responsible.Contains(filter.Responsible));
            }
            if (filter.Time != 0)
            {
                ret = ret.Where(c => c.mePa.Time == filter.Time);
            }


            var result = await ret.Select(c => new PatientFilterDTO
            {
                Name = c.pa.Name,
                Illness = c.ill.Name,
                Medicine = c.me.Name,
                Dosage = c.mePa.Dosage,
                Responsible = c.pa.Responsible,
                Time = c.mePa.Time
            }).ToListAsync();

            return result;
        }
      
        public async Task<List<Patient>> GetPatientById(long id)
        {
            return await this.FindBy(c => c.Id == id);
        }
        public async Task CreatePatient (Patient patient)
        {
            await this.Create(patient);
        }

        public async Task UpdatePatient(Patient patient)
        {
            await this.Update(patient);
        }

        public async Task DeletePatient (Patient patient)
        {
            await this.Delete(patient);
        }
        public async Task GetPatientByAny(string patient)
        {
            await this.Any(c => c.Name == patient);
        }

    }
}
