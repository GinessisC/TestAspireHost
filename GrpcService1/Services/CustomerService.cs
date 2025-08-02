using Grpc.Core;
using GrpcService1.Data;
using GrpcService1.Models;

namespace GrpcService1.Services;

public class CustomerService : Customer.CustomerBase
{
	private readonly IDataProvider _dataProvider;

	public CustomerService(IDataProvider dataProvider)
	{
		_dataProvider = dataProvider;
	}

	public override async Task<CustomerModel> GetCustomerInfo(CustomerLookUpModel request, ServerCallContext context)
	{
		CustomerDto customer = await _dataProvider.GetCustomer(request.UserId);

		return new CustomerModel
		{
			Age = customer.Age,
			CustomerDetails = new CustomerPrivateDetails
			{
				Email = customer.Email,
				FirstName = customer.FirstName
			}
		};
	}

	public override async Task GetAllCustomers(
		GetAllCustomersRequest request,
		IServerStreamWriter<CustomerModel> stream,
		ServerCallContext context)
	{
		IList<CustomerDto> customers = await _dataProvider.GetAllCustomersAsync();

		foreach (CustomerDto customerDto in customers)
		{
			CustomerModel model = MapToCustomerModel(customerDto);
			await stream.WriteAsync(model);
		}
	}

	private CustomerModel MapToCustomerModel(CustomerDto customerDto)
	{
		return new CustomerModel
		{
			Age = customerDto.Age,
			CustomerDetails = new CustomerPrivateDetails
			{
				Email = customerDto.Email,
				FirstName = customerDto.FirstName
			}
		};
	}
}