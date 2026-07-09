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
    public class AccessLogRepository : BaseRepository<AccessLog>, IAccessLogRepository
    {
        private readonly SistemaDeCadastroContext _context;

        public AccessLogRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AccessLog?> GetById(long id)
        {
            return await _context.AccessLogs.FindAsync(id);
        }

        public async Task<List<AccessLog>> GetByUserId(long userId)
        {
            return await _context.AccessLogs
                                 .Where(a => a.UserId == userId)
                                 .ToListAsync();
        }
    }
}
