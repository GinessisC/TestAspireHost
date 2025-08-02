using DataProvider.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataProvider;

public class AppDbContext : DbContext
{
	public DbSet<MessageEntity> Messages { get; set; }
	public DbSet<UserEntity> Users { get; set; }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
}