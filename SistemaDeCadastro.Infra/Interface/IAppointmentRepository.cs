using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        /// <summary>Busca um agendamento pelo id ja convertido em DTO com o nome do acolhido (null se nao existir).</summary>
        Task<AppointmentDTO?> GetById(long id);
        /// <summary>Busca a entidade do agendamento carregando junto o acolhido e o usuario.</summary>
        Task<Appointment?> GetWithPatientAndUser(long id);
        /// <summary>Lista todos os agendamentos de um acolhido.</summary>
        Task<List<Appointment>> GetByPatientId(long patientId);
        /// <summary>Lista paginada de agendamentos com o nome do acolhido, aplicando os filtros informados.</summary>
        Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter);
        /// <summary>Lista todos os agendamentos (sem paginacao) da instituicao do usuario logado.</summary>
        Task<List<AppointmentListDTO>> GetAllAppointment();

    }
}
