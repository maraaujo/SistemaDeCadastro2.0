using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
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
        public async Task<PagedAcesseLog> GetAccessLogsByFilter(AccessLogFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;

            var query = from a in _context.AccessLogs
                        join u in _context.LoginAccounts on a.UserId equals u.UserId
                        select new AccessLogListDTO
                        {
                            Id = a.Id,
                            UserEmail = u.Email,
                            UserType = u.UserType,
                            Action = a.Action,
                            DateTime = a.DateTime
                        };

            if (!string.IsNullOrEmpty(filter.UserEmail))
            {
                query = query.Where(a => a.UserEmail.Contains(filter.UserEmail));
            }

            if (!string.IsNullOrEmpty(filter.Action))
            {
                query = query.Where(a => a.Action.Contains(filter.Action));
            }

            if (filter.DateTime.HasValue)
            {
                var startDate = filter.DateTime.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.DateTime >= startDate && a.DateTime < endDate);
            }

            var ret = new PagedAcesseLog();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.AccessLogs = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }

    }
}
