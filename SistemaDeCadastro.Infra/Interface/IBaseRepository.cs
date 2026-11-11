using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>Insere a entidade no banco e salva imediatamente.</summary>
        Task Create(T entity);
        /// <summary>Retorna todos os registros que atendem a condicao informada.</summary>
        Task<List<T>> FindBy(Expression<Func<T, bool>> expression);
        /// <summary>Retorna todos os registros da tabela (respeitando o filtro por instituicao, quando houver).</summary>
        Task<List<T>> GetAll();
        /// <summary>Indica se existe pelo menos um registro que atende a condicao.</summary>
        Task<bool> Any(Expression<Func<T, bool>> predicate);
        /// <summary>Remove a entidade do banco e salva imediatamente.</summary>
        Task Delete(T entity);
        /// <summary>Remove varias entidades de uma vez e salva imediatamente.</summary>
        Task DeleteRange(IEnumerable<T> entity);
        /// <summary>Marca a entidade como alterada e grava todas as colunas no banco.</summary>
        Task Update(T entity);
    }
}
