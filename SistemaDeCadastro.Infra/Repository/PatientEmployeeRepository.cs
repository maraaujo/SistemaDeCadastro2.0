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
    public class PatientEmployeeRepository : BaseRepository<PatientEmployee>, IPatientEmployeeRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public PatientEmployeeRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>Busca o vinculo acolhido x funcionario pelo id (somente de acolhidos da instituicao logada).</summary>
        public async Task<PatientEmployee?> GetById(long id)
        {
            return await _context.PatientEmployees.FirstOrDefaultAsync(pe => pe.Id == id);
        }

        /// <summary>Lista todos os vinculos de funcionarios responsaveis pelo acolhido informado.</summary>
        public async Task<List<PatientEmployee>> GetByPatientId(long patientId)
        {
            return await _context.PatientEmployees.Where(pe => pe.PatientId == patientId).ToListAsync();
        }
        /// <summary>
        /// Lista paginada dos vinculos acolhido x funcionario. Junta PatientEmployees com Patients e Employees
        /// para trazer os nomes e o cargo do funcionario (exibido como funcao de responsabilidade). Filtros
        /// opcionais: id e trecho do nome do acolhido, id e trecho do nome do funcionario, trecho da funcao,
        /// dia de inicio e dia de fim. Ordena do maior id para o menor, 15 por pagina.
        /// Obs.: o EmployeeId nao e preenchido na projecao, entao o filtro por EmployeeId pode nao funcionar.
        /// </summary>
        public async Task<PagedPatientEmployeeDTO> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            
            var query = from pe in _context.PatientEmployees
                        join p in _context.Patients on pe.PatientId equals p.Id
                        join e in _context.Employees on pe.EmployeeId equals e.Id
                        select new PatientEmployeeListDTO
                        {
                            Id = pe.Id,
                            PatientId = p.Id,
                            PatientName = p.Name,
                            EmployeeName = e.Name,
                            ResponsibilityFunction = e.Position,
                            StartDate = pe.StartDate,
                            EndDate = pe.EndDate
                        };
            if(filter.PatientId.HasValue)
            {
                query = query.Where(pe => pe.PatientId == filter.PatientId.Value);
            }
            if(!string.IsNullOrEmpty(filter.PatientName))
            {
                query = query.Where(pe => pe.PatientName.Contains(filter.PatientName));
            }
            if(filter.EmployeeId.HasValue)
            {
                query = query.Where(pe => pe.EmployeeId == filter.EmployeeId.Value);
            }
            if(!string.IsNullOrEmpty(filter.EmployeeName))
            {
                query = query.Where(pe => pe.EmployeeName.Contains(filter.EmployeeName));
            }
            if (!string.IsNullOrEmpty(filter.ResponsibilityFunction))
            {
                query = query.Where(pe => pe.ResponsibilityFunction.Contains(filter.ResponsibilityFunction));
            }
            if(filter.StartDate.HasValue)
            {
                var startDate = filter.StartDate.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.StartDate >= startDate && a.StartDate < endDate);
            }
            if(filter.EndDate.HasValue)
            {
                var startDate = filter.EndDate.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.EndDate >= startDate && a.EndDate < endDate);
            }
            var ret = new PagedPatientEmployeeDTO();
            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.PatientEmployees = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
