using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(SistemaCadastroContext context)
            : base(context)
        {
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
