﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Data.Interface
{
    public interface IBaseRepository<T>
    {
        Task Create(T entity);
        Task<List<T>> FindBy(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAll();
        Task Delete(T entity);
        Task Update(T entity);
    }
}
