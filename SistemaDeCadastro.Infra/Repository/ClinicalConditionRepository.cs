using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class ClinicalConditionRepository : BaseRepository<ClinicalCondition>, IClinicalConditionRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public ClinicalConditionRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ClinicalCondition?> GetById(long id)
        {
            return await _context.ClinicalConditions.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<PagedClinicalConditionDTO> GetClinicalConditionByFilter(ClinicalConditionFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from c in _context.ClinicalConditions
                        select new ClinicalConditionListDTO
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Type = c.Type
                        };
                       
         
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(c => c.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(c => c.Description.Contains(filter.Description));
            }
            if (!string.IsNullOrEmpty(filter.Type))
            {
                query = query.Where(c => c.Type.Contains(filter.Type));
            }   
               
            var ret = new PagedClinicalConditionDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.ClinicalConditions = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
