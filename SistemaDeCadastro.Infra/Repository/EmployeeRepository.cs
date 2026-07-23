using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public EmployeeRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee?> GetById(long id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Employee>> GetByDepartmentId(long departmentId)
        {
            return await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
        }
        public async Task<PagedEmployeeDTO> GetEmployeeByFilter(EmployeeFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from e in _context.Employees
                        join d in _context.Departments on e.DepartmentId equals d.Id
                        select new EmployeeListDTO
                        {
                            Id = e.Id,
                            Name = e.Name,
                            Cpf = e.Cpf,
                            Email = e.Email,
                            DepartmentId = e.DepartmentId,
                            DepartmentName = d.Name,
                            Phone = e.Phone,
                            Position = e.Position,
                            AdmissionDate = e.AdmissionDate,
                        };
            if(!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(e => e.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.Cpf))
            {
                query = query.Where(e => e.Cpf.Contains(filter.Cpf));
            }
            if (!string.IsNullOrEmpty(filter.Position))
            {
                query = query.Where(e => e.Position.Contains(filter.Position));
            }
            if (!string.IsNullOrEmpty(filter.Phone))
            {
                query = query.Where(e => e.Phone.Contains(filter.Phone));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(e => e.Email.Contains(filter.Email));
            }
            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == filter.DepartmentId.Value);
            }
            if (filter.AdmissionDate.HasValue)
            {
                var startDate = filter.AdmissionDate.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.AdmissionDate >= startDate && a.AdmissionDate < endDate);
            }
            var ret = new PagedEmployeeDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Employees = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
