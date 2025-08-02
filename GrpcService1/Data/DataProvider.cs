using GrpcService1.Models;

namespace GrpcService1.Data;

public class DataProvider : IDataProvider
{
	private readonly List<CustomerDto> customers =
	[
		new(1, "Unknown", "@gmail.com", 33),
		new(2, "Lol", "lol@gmail.com", 55)
	];

	public async Task<CustomerDto> GetCustomer(int customerId)
	{
		await Task.Delay(100);

		CustomerDto? customer = customers.FirstOrDefault(c => c.UserId == customerId)
			?? throw new InvalidOperationException("Customer not found");

		return customer;
	}

	public async Task<IList<CustomerDto>> GetAllCustomersAsync()
	{
		return await Task.FromResult(customers);
	}
}