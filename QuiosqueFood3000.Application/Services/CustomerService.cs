using System.Text.Json;
using System.Text;
using QuiosqueFood3000.Api.DTOs;
using QuiosqueFood3000.Api.Services.Interfaces;
using QuiosqueFood3000.Api.Validators;
using QuiosqueFood3000.Domain.Entities;
using QuiosqueFood3000.Infraestructure.Repositories.Interfaces;


namespace QuiosqueFood3000.Application.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<CustomerDto?> GetCustomerByCpf(string cpf)
    {
        var httpClient = new HttpClient();
        var response = httpClient.PostAsync(
            "https://us-central1-quiosquefood3000.cloudfunctions.net/get-customer?cpf=" + cpf,
            null
        ).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao chamar função externa: {response.StatusCode}");
        }
        var responseBody = await response.Content.ReadAsStringAsync();

        var responseBodyDeserialized = JsonSerializer.Deserialize<JsonDocument>(responseBody);

        var root = responseBodyDeserialized?.RootElement;
        if (root == null || root?.ValueKind != JsonValueKind.Array || root?.GetArrayLength() == 0)
        {
            throw new Exception("Resposta da função externa não contém dados de cliente.");
        }

        // Fix: Ensure `root` is not nullable by using `.Value` before indexing
        var customerElement = root.Value[0];
        CustomerDto customerDto = new CustomerDto()
        {
            Cpf = customerElement.GetProperty("cpf").GetString(),
            Name = customerElement.GetProperty("nome").GetString(),
            Email = customerElement.GetProperty("email").GetString()
        };

        CustomerDtoValidator customerDtoValidator = new CustomerDtoValidator();
        var resultCustomerDto = customerDtoValidator.Validate(customerDto);

        if (!resultCustomerDto.IsValid)
        {
            throw new InvalidDataException(resultCustomerDto.ToString());
        }

        return customerDto;
    }
    public CustomerDto RegisterCustomer(CustomerDto customerDto)
    {
        CustomerDtoValidator customerDtoValidator = new CustomerDtoValidator();
        var resultCustomerDto = customerDtoValidator.Validate(customerDto);

        if (!resultCustomerDto.IsValid)
        {
            throw new InvalidDataException(resultCustomerDto.ToString());
        }

        Customer customer = new Customer()
        {
            Cpf = customerDto.Cpf,
            Name = customerDto.Name,
            Email = customerDto.Email
        };
        CustomerValidator customerValidator = new CustomerValidator();
        var resultCustomer = customerValidator.Validate(customer);

        if (!resultCustomer.IsValid)
        {
            throw new InvalidDataException(resultCustomer.ToString());
        }
        // Chamada HTTP para a função externa
        var payload = new
        {
            nome = customerDto.Name,
            email = customerDto.Email,
            cpf = customerDto.Cpf
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8,
        "application/json");
        using var httpClient = new HttpClient();
        var response = httpClient.PostAsync(
            "https://us-central1-quiosquefood3000.cloudfunctions.net/create-customer",
            content
        ).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao chamar função externa: {response.StatusCode}");
        }

        customerDto = new CustomerDto()
        {
            Cpf = customer.Cpf,
            Name = customer.Name,
            Email = customer.Email
        };
        return customerDto;
    }
    public void RemoveCustomer(CustomerDto customerDto)
    {
        if (string.IsNullOrWhiteSpace(customerDto.Id))
        {
            throw new ArgumentNullException("O Id do cliente deve ser informado");
        }
        Customer customer = new Customer()
        {
            Id = int.Parse(customerDto.Id),
            Cpf = customerDto.Cpf,
            Name = customerDto.Name,
            Email = customerDto.Email
        };
        customerRepository.RemoveCustomer(customer);
    }
    public CustomerDto UpdateCustomer(CustomerDto customerDto)
    {

        if (customerDto == null)
        {
            throw new ArgumentNullException("O cliente não pode ser nulo");
        }

        if (string.IsNullOrWhiteSpace(customerDto.Id))
        {
            throw new ArgumentNullException("O Id do cliente deve ser informado");
        }
        CustomerDtoValidator customerDtoValidator = new CustomerDtoValidator();
        var resultCustomerDto = customerDtoValidator.Validate(customerDto);

        if (!resultCustomerDto.IsValid)
        {
            throw new InvalidDataException(resultCustomerDto.ToString());
        }

        Customer customer = new Customer()
        {
            Id = int.Parse(customerDto.Id),
            Cpf = customerDto.Cpf,
            Name = customerDto.Name,
            Email = customerDto.Email
        };
        CustomerValidator customerValidator = new CustomerValidator();
        var resultCustomer = customerValidator.Validate(customer);

        if (!resultCustomer.IsValid)
        {
            throw new InvalidDataException(resultCustomer.ToString());
        }
        customer = customerRepository.UpdateCustomer(customer);
        customerDto = new CustomerDto()
        {
            Id = customer.Id.ToString(),
            Cpf = customer.Cpf,
            Name = customer.Name,
            Email = customer.Email
        };
        return customerDto;
    }
}