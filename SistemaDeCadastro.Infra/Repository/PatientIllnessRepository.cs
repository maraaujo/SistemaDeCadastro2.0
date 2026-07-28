using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.Filters;

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
        public async Task<PagedPatientIllnessDTO> GetPatientIllnessByFilter(PatientIllnessFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from pi in _context.PatientIllnesses
                        join p in _context.Patients on pi.PatientId equals p.Id
                        join i in _context.Illnesses on pi.IllnessId equals i.Id
                        select new PatientIllnessListDTO
                        {
                            Id = pi.Id,
                            PatientId = p.Id,
                            PatientName = p.Name,
                            IllnessId = i.Id,
                            IllnessName = i.Name,
                            DiagnosisDate = pi.DiagnosisDate,
                            Observations = pi.Observations
                        };
            if (filter.Id.HasValue)
            {
                query = query.Where(pi => pi.Id == filter.Id.Value);
            }
            if (filter.PatientId.HasValue)
            {
                query = query.Where(pi => pi.PatientId == filter.PatientId.Value);
            }
            if (filter.IllnessId.HasValue)
            {
                query = query.Where(pi => pi.IllnessId == filter.IllnessId.Value);
            }
            if (!string.IsNullOrEmpty(filter.PatientName))
            {
                query = query.Where(pi => pi.PatientName.Contains(filter.PatientName));
            }
            if (!string.IsNullOrEmpty(filter.IllnessName))
            {
                query = query.Where(pi => pi.IllnessName.Contains(filter.IllnessName));
            }
            var ret = new PagedPatientIllnessDTO();
            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.PatientIllnesses = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
