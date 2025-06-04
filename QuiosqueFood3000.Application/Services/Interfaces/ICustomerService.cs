using QuiosqueFood3000.Api.DTOs;

namespace QuiosqueFood3000.Api.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetCustomerByCpf(string cpf);
    Task<CustomerDto> RegisterCustomer(CustomerDto customerDto);
    void RemoveCustomer(CustomerDto customerDto);
    Task<CustomerDto> UpdateCustomer(CustomerDto customerDto);
}