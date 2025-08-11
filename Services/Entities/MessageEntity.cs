namespace Services.Entities;

public class MessageEntity
{
	public int IssuerId { get; set; }
	public int ReceiverId { get; set; }
	public string TextMessage { get; set; } = string.Empty;
}