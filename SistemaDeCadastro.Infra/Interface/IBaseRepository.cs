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
        Task Create(T entity);
        Task<List<T>> FindBy(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAll();
        Task Delete(T entity);
        Task DeleteRange(IEnumerable<T> entity);
        Task Update(T entity);
    }
}
