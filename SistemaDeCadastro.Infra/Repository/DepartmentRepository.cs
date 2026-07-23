using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;


namespace SistemaDeCadastro.Infra.Repository
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public DepartmentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Department?> GetById(long id)
        {
            return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<PagedDepartmentDTO> GetDepartmentByFilter(DepartmentFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from d in _context.Departments
                        select new DepartmentListDTO
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Description = d.Description
                        };
            if(!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(d => d.Name.Contains(filter.Name));
            }
            if(!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(d => d.Description.Contains(filter.Description));
            }
            var ret = new PagedDepartmentDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Departments = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;

        }
    }
}
