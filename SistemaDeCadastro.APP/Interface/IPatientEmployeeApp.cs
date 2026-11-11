using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
namespace SistemaDeCadastro.APP.Interface
{
    public interface IPatientEmployeeApp
    {
        /// <summary>Devolve todos os vinculos acolhido-funcionario da instituicao.</summary>
        Task<List<PatientEmployee>> GetAll();
        /// <summary>Devolve o vinculo acolhido-funcionario do Id informado, ou null se nao existir.</summary>
        Task<PatientEmployee?> GetById(long id);
        /// <summary>Cria o vinculo de um funcionario responsavel por um acolhido.</summary>
        Task<ApiResponse> Create(CreatePatientEmployeeDTO entity);
        /// <summary>Atualiza um vinculo acolhido-funcionario existente.</summary>
        Task<ApiResponse> Update(UpdatePatientEmployeeDTO entity);
        /// <summary>Exclui o vinculo acolhido-funcionario do Id informado.</summary>
        Task<ApiResponse> Delete(long id);
        /// <summary>Devolve a lista paginada de vinculos acolhido-funcionario conforme o filtro.</summary>
        Task<PagedPatientEmployeeDTO> GetPagedPatientEmployeeByFilter(PatientEmployeeFilterDTO filter);
    }
}
