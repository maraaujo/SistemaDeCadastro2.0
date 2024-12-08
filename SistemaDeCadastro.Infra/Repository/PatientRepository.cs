using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        private readonly IMedicinePatientIllnessRepository _medicinePatientIllnessRepository;
        private readonly IPatientIllnessRepository _patientIllnessRepository;
        private readonly Domain.SistemaCadastroContext.SistemaCadastroContext _context;
        public PatientRepository(Domain.SistemaCadastroContext.SistemaCadastroContext context,
            IMedicinePatientIllnessRepository medicinePatientIllnessRepository,
            IPatientIllnessRepository patientIllnessRepository)
            : base(context)
        {
            this._medicinePatientIllnessRepository = medicinePatientIllnessRepository;
            this._patientIllnessRepository = patientIllnessRepository;
            this._context = context;
        }

        public async Task<List<PatientFilterDTO>> FilterPatient(PatientFilterDTO filter)
        {
            var ret = (from pa in _context.Patients
                          join paIll in _context.PatientIllnesses
                              on pa.Id equals paIll.IdPatient
                          join ill in _context.Illnesses
                              on paIll.IdIlleness equals ill.Id
                          join mePa in _context.MedicinePatientIllnesses
                              on paIll.Id equals mePa.IdPatientIllness
                          join me in _context.Medicines
                              on mePa.IdMedicine equals me.Id
                          select new
                          {
                                    pa,
                                    paIll,
                                    ill,
                                    mePa,
                                    me,
                                });
            /*
             * Nome patient
             * Resposible Patient
             * 
             */
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
