using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbContext _context;

        /// <summary>Guarda o DbContext usado por todas as operacoes; lanca ArgumentNullException se vier nulo.</summary>
        public BaseRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>Adiciona a entidade na tabela de T e chama SaveChanges na hora (o id gerado fica na propria entidade).</summary>
        public async Task Create(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Executa um SELECT com o WHERE informado e devolve a lista de resultados.
        /// O filtro por instituicao do contexto e aplicado automaticamente nas entidades que o possuem.
        /// </summary>
        public async Task<List<T>> FindBy(Expression<Func<T, bool>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        /// <summary>Retorna todos os registros da tabela de T (ainda sujeitos ao filtro por instituicao, quando houver).</summary>
        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>Remove a entidade e chama SaveChanges na hora.</summary>
        public async Task Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        /// <summary>Remove todas as entidades da colecao em uma unica gravacao (um SaveChanges).</summary>
        public async Task DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        /// <summary>Retorna true se existir pelo menos um registro que atenda a condicao (SELECT EXISTS).</summary>
        public async Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await _context.Set<T>().AnyAsync(predicate);
        }

        /// <summary>
        /// Marca a entidade inteira como Modified e chama SaveChanges: todas as colunas sao
        /// gravadas com os valores atuais do objeto (nao so as que mudaram).
        /// </summary>
        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
