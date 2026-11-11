using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientApp
    {
        /// <summary>Devolve uma lista com o acolhido do Id informado.</summary>
        Task<List<Patient>> GetPatientById(long id);
        /// <summary>Cadastra um acolhido com responsaveis, condicoes clinicas e medicamentos programados.</summary>
        Task<ApiResponse> CreatePatient(CreatePatientDTO patient);
        /// <summary>Atualiza o acolhido e cria/atualiza seus responsaveis, condicoes e medicamentos.</summary>
        Task<ApiResponse> UpdatePatient(PatientDTO patient);
        /// <summary>Exclui o acolhido e seus dependentes (agendamentos, pagamentos, condicoes, responsaveis).</summary>
        Task<ApiResponse> DeletePatient(long id);
        /// <summary>Executa a busca de acolhidos por texto (nao devolve resultado).</summary>
        Task GetPatientByAny(string patient);
        /// <summary>Devolve a lista paginada de acolhidos conforme o filtro.</summary>
        Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter);
        /// <summary>Devolve a ficha completa do acolhido, ou null se nao existir.</summary>
        Task<DetailsPatientDTO?> DetailsPatient(long id);
        /// <summary>Devolve todos os acolhidos da instituicao do usuario.</summary>
        Task<List<Patient>> GetAllPatients();

    }
}
