using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IAppointmentApp
    {
        /// <summary>Devolve todos os agendamentos da instituicao do usuario, no formato de listagem.</summary>
        Task<List<AppointmentListDTO>> GetAllAppointment() ;
        /// <summary>Devolve o agendamento do Id informado, ou null se nao existir.</summary>
        Task<AppointmentDTO?> GetById(long id);
        /// <summary>Cria um agendamento para um acolhido da instituicao do usuario logado.</summary>
        Task<ApiResponse> Create(CreateAppointmentDTO entity);
        /// <summary>Atualiza um agendamento existente (acolhido, tipo, data/hora, responsavel, status, observacoes).</summary>
        Task<ApiResponse> Update(UpdateAppointmentDTO entity);
        /// <summary>Exclui o agendamento do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de agendamentos conforme o filtro.</summary>
        Task<PagedAppointmentDTO> GetPagedAppointmentByFilter(AppointmentFilterDTO filter);

    }
}
