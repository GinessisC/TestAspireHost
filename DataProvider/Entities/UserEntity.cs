using System.ComponentModel.DataAnnotations;

namespace DataProvider.Entities;

public class UserEntity
{
	public string UserName { get; set; } = string.Empty;

	[Key]
	public int UserId { get; set; }

	public string Description { get; set; } = string.Empty;
}