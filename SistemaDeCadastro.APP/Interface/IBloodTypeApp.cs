using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IBloodTypeApp
    {
        /// <summary>Devolve uma lista com o tipo sanguineo do Id informado.</summary>
        Task<List<BloodType>> GetBloodTypeById(long id);
        /// <summary>Devolve em Data todos os tipos sanguineos cadastrados.</summary>
        Task<ApiResponse> GetAllBloodIllness();
        /// <summary>Cadastra um tipo sanguineo.</summary>
        Task<ApiResponse> CreateBloodType(BloodTypeDTO bloodType);
        /// <summary>Exclui o tipo sanguineo cujo Id vem no DTO.</summary>
        Task<ApiResponse> DeleteIllness(BloodTypeDTO bloodType);

    }
}
