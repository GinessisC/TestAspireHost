using GrpcService1.Models;

namespace GrpcService1.Data;

public interface IDataProvider
{
	Task<CustomerDto> GetCustomer(int customerId);
	Task<IList<CustomerDto>> GetAllCustomersAsync();
}