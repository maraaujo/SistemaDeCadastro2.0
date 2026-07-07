using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientIllnessRepository : BaseRepository<PatientIllness>, IPatientIllnessRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public PatientIllnessRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PatientIllness?> GetById(long id)
        {
            return await _context.PatientIllnesses.FirstOrDefaultAsync(pi => pi.Id == id);
        }
    }
}
