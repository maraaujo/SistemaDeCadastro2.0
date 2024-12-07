using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientIllnessRepository : BaseRepository<PatientIllness>,IPatientIllnessRepository
    {
        public PatientIllnessRepository(DbContext context) : base(context)
        {
        }
        public async Task CreatePatientIllness (PatientIllness patientIllness)
        {
            await this.Create(patientIllness);
        }
    }
}
