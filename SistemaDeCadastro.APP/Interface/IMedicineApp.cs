using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IMedicineApp
    {
        /// <summary>Devolve uma lista com o medicamento do Id informado.</summary>
        Task<List<Medicine>> GetMedicineId(long id);
        /// <summary>Devolve todos os medicamentos da instituicao do usuario.</summary>
        Task<List<Medicine>> GetAll();
        /// <summary>Cadastra um medicamento na instituicao do usuario logado.</summary>
        Task<ApiResponse> CreateMedicine(CreateMedicineDTO medicine);
        /// <summary>Atualiza nome, descricao, dosagem e via de administracao de um medicamento.</summary>
        Task<ApiResponse> UpdateMedicine(UpdateMedicineDTO medicine);
        /// <summary>Exclui o medicamento cujo Id vem no DTO.</summary>
        Task<ApiResponse> DeleteMedicine(MedicineDTO medicine);
        /// <summary>Devolve a lista paginada de medicamentos conforme o filtro.</summary>
        Task<PagedMedicineDTO> GetMedicineByFilter(MedicineFilterDTO filter);
    }
}
