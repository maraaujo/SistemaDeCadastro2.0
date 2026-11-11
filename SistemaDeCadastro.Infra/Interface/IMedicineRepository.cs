using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;


namespace SistemaDeCadastro.Infra.Interface
{
    public interface IMedicineRepository : IBaseRepository<Medicine>
    {
        /// <summary>Busca o medicamento pelo id (lista com 0 ou 1 item, somente da instituicao do usuario logado).</summary>
        Task<List<Medicine>> GetMedicineById(long ind);
        /// <summary>Verifica se existe medicamento com o nome exato, mas o resultado nao e devolvido (retorno Task sem valor).</summary>
        Task GetMedicineByAnyValorString(string medicine);
        /// <summary>Cadastra um medicamento.</summary>
        Task CreateMedicine(Medicine medicine);
        /// <summary>Atualiza um medicamento.</summary>
        Task UpdateMedicine(Medicine medicine);
        /// <summary>Exclui um medicamento.</summary>
        Task DeleteMedicine(Medicine medicine);
        /// <summary>Lista paginada de medicamentos filtrando por id, nome, descricao, dosagem e via de administracao.</summary>
        Task<PagedMedicineDTO> GetMedicineByFilter(MedicineFilterDTO filter);
    }
}
