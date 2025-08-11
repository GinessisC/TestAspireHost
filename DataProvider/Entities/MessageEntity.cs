using System.ComponentModel.DataAnnotations;

namespace DataProvider.Entities;

public class MessageEntity
{
	[Key]
	public int MessageId { get; set; }

	public int ReceiverId { get; set; }
	public string MessageText { get; set; } = string.Empty;
	public int SenderId { get; set; }
}