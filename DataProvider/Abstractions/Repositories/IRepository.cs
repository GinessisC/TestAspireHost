using System.Linq.Expressions;

namespace DataProvider.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
	Task AddAsync(TEntity entity);
	IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
	Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
}