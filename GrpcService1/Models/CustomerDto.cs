namespace GrpcService1.Models;

public class CustomerDto
{
	public int UserId { get; set; }
	public string FirstName { get; set; }
	public string Email { get; set; }
	public int Age { get; set; }

	public CustomerDto(int userId,
		string firstName,
		string email,
		int age)
	{
		UserId = userId;
		FirstName = firstName;
		Email = email;
		Age = age;
	}
}