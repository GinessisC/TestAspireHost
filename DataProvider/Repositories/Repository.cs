using System.Linq.Expressions;
using DataProvider.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataProvider.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
	where TEntity : class
{
	private readonly AppDbContext _dbContext;
	private readonly DbSet<TEntity> _dbSet;
	private readonly ILogger<Repository<TEntity>> _logger;

	public Repository(AppDbContext dbContext,
		ILogger<Repository<TEntity>> logger
	)
	{
		_dbContext = dbContext;
		_logger = logger;
		_dbSet = _dbContext.Set<TEntity>();
	}

	public async Task AddAsync(TEntity entity, CancellationToken ct)
	{
		_dbContext.Add(entity);
		await _dbContext.SaveChangesAsync(ct);
	}

	//Is it not dangerous to use such non-asynchronous method?
	public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
	{
		return _dbSet.Where(predicate);
	}

	public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
	{
		ArgumentNullException.ThrowIfNull(predicate);
		TEntity? entity = await _dbSet.FirstOrDefaultAsync(predicate, ct);
		
		if (entity is null)
		{
			_logger.LogTrace("No requested entity found");
		}

		return entity;
	}
}