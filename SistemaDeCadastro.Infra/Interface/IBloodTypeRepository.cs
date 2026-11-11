using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IBloodTypeRepository : IBaseRepository<BloodType>
    {
        /// <summary>Busca o tipo sanguineo pelo id (lista com 0 ou 1 item).</summary>
        Task<List<BloodType>> GetBloodTypeById(long id);
        /// <summary>Lista todos os tipos sanguineos cadastrados.</summary>
        Task<List<BloodType>> FindAll();
        /// <summary>Cadastra um tipo sanguineo e devolve a entidade ja com o id gerado.</summary>
        Task<BloodType> CreateBooldType(BloodType booldtype);
        /// <summary>Exclui o tipo sanguineo informado (recebe a entidade, apesar do nome do parametro).</summary>
        Task DeleteBloodtype(BloodType id); 
    }
}
