using Azure;
using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
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
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public AppointmentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Lista todos os agendamentos (sem paginacao nem ordenacao) ja convertidos em DTO, com o nome
        /// do acolhido vindo da navegacao Patient. Restrito a instituicao do usuario logado.
        /// </summary>
        public async Task<List<AppointmentListDTO>> GetAllAppointment()
        {
            var ret = await (from a in _context.Appointments
                      select new AppointmentListDTO
                      {
                          Id = a.Id,
                          PatientId = a.PatientId,
                          PatientName = a.Patient.Name,
                          UserId = a.UserId,
                          AppointmentType = a.AppointmentType,
                          Observations = a.Observations,
                          DateTime = a.DateTime,
                          Responsible = a.Responsible,
                          Status = a.Status
                      }).ToListAsync();

            return  ret;
        } 

        /// <summary>Busca um agendamento pelo id e devolve o DTO com o nome do acolhido (null se nao existir ou for de outra instituicao).</summary>
        public async Task<AppointmentDTO> GetById(long id)
        {
            var ret = await (from a in _context.Appointments
                      where a.Id == id
                      select new AppointmentDTO
                      {
                          Id = a.Id,
                          PatientId = a.PatientId,
                          PatientName = a.Patient.Name,
                          UserId = a.UserId,
                          AppointmentType = a.AppointmentType,
                          Observations = a.Observations,
                          DateTime = a.DateTime,
                          Responsible = a.Responsible,
                          Status = a.Status
                      }).FirstOrDefaultAsync();

            return  ret ;
        }

        /// <summary>
        /// Lista paginada de agendamentos. Junta Appointments com Patients para trazer o nome do acolhido.
        /// Filtros opcionais: id do acolhido, dia da data/hora e trecho (sem diferenciar maiusculas) de nome
        /// do acolhido, tipo, responsavel, status e observacoes. O UserId do filtro nao e usado.
        /// Ordena do maior id para o menor, 15 por pagina, ja restrito a instituicao logada.
        /// </summary>
        public async Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;

            var query = from a in _context.Appointments
                         join p in _context.Patients on a.PatientId equals p.Id
                        select new AppointmentFilterDTO
                        {
                            Id = a.Id,
                            PatientId = a.PatientId,
                            PatientName = p.Name,
                            UserId = a.UserId,
                            AppointmentType = a.AppointmentType,
                            Observations = a.Observations,
                            DateTime = a.DateTime,
                            Responsible = a.Responsible,
                            Status = a.Status
                        };
            if(!string.IsNullOrEmpty(filter.PatientName))
            {
                query = query.Where(a => a.PatientName.ToLower().Contains(filter.PatientName.ToLower()));
            }
            if(filter.PatientId.HasValue)
            {
                query = query.Where(a => a.PatientId == filter.PatientId.Value);
            }
            if(!string.IsNullOrEmpty(filter.AppointmentType))
            {
                query = query.Where(a => a.AppointmentType.ToLower().Contains(filter.AppointmentType.ToLower()));
            }
            if (filter.DateTime.HasValue)
            {
                var startDate = filter.DateTime.Value.Date;
                var endDate = startDate.AddDays(1);

                query = query.Where(a => a.DateTime >= startDate && a.DateTime < endDate);
            }
            if (!string.IsNullOrEmpty(filter.Responsible))
            {
                query = query.Where(a => a.Responsible.ToLower().Contains(filter.Responsible.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(a => a.Status.ToLower().Contains(filter.Status.ToLower()));
            }
            if (!string.IsNullOrEmpty(filter.Observations))
            {
                query = query.Where(a => a.Observations.ToLower().Contains(filter.Observations.ToLower()));
            }
            var ret = new PagedAppointmentDTO();
            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Appointments = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;

        }
        /// <summary>Busca a entidade do agendamento pelo id carregando junto o acolhido e o usuario (conta de acesso).</summary>
        public async Task<Appointment?> GetWithPatientAndUser(long id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>Lista todos os agendamentos do acolhido informado.</summary>
        public async Task<List<Appointment>> GetByPatientId(long patientId)
        {
            return await _context.Appointments.Where(a => a.PatientId == patientId).ToListAsync();
        }
    }
}
