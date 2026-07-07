using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
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
    }
}
