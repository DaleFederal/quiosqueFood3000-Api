using System.Text.Json;
using System.Text;
using QuiosqueFood3000.Api.DTOs;
using QuiosqueFood3000.Api.Services.Interfaces;
using QuiosqueFood3000.Api.Validators;
using QuiosqueFood3000.Domain.Entities;
using System.Net.Http.Headers;
using QuiosqueFood3000.Api.Helpers;


namespace QuiosqueFood3000.Application.Services;

public class CustomerService() : ICustomerService
{
    public async Task<CustomerDto?> GetCustomerByCpf(string cpf)
    {
        var httpClient = new HttpClient();
        string bearerToken = await GoogleCloudHelper.GetBearerToken("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyBcWXnfmt7rN3JQ-b14brXypwg9Wmeu7Ow");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var response = httpClient.PostAsync(
            "https://us-central1-quiosquefood3000.cloudfunctions.net/get-customer?cpf=" + cpf,
            null
        ).GetAwaiter().GetResult();
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao chamar consulta de cliente: {response.StatusCode}");
        }
        var responseBody = await response.Content.ReadAsStringAsync();

        var responseBodyDeserialized = JsonSerializer.Deserialize<JsonDocument>(responseBody);

        var root = responseBodyDeserialized?.RootElement;
        if (root == null || root?.ValueKind != JsonValueKind.Array || root?.GetArrayLength() == 0)
        {
            throw new Exception("Cliente não encontrado");
        }
        var customerElement = root.Value[0];

        if (root == null || root?.ValueKind != JsonValueKind.Array || root?.GetArrayLength() == 0)
        {
            throw new Exception("Cliente não encontrado.");
        }

        CustomerDto customerDto = new CustomerDto()
        {
            Cpf = customerElement.GetProperty("cpf").GetString(),
            Name = customerElement.GetProperty("name").GetString(),
            Email = customerElement.GetProperty("email").GetString(),
            Id = customerElement.GetProperty("id").GetInt32().ToString()
        };

        CustomerDtoValidator customerDtoValidator = new CustomerDtoValidator();
        var resultCustomerDto = customerDtoValidator.Validate(customerDto);

        if (!resultCustomerDto.IsValid)
        {
            throw new InvalidDataException(resultCustomerDto.ToString());
        }

        return customerDto;
    }
    public async Task<CustomerDto> RegisterCustomer(CustomerDto customerDto)
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
        var payload = new
        {
            name = customerDto.Name,
            email = customerDto.Email,
            cpf = customerDto.Cpf
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8,
        "application/json");

        using var httpClient = new HttpClient();

        string bearerToken = await GoogleCloudHelper.GetBearerToken("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyBcWXnfmt7rN3JQ-b14brXypwg9Wmeu7Ow");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

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
    public async void RemoveCustomer(CustomerDto customerDto)
    {
        if (string.IsNullOrWhiteSpace(customerDto.Cpf))
        {
            throw new ArgumentNullException("O Cpf do cliente deve ser informado");
        }

        using var httpClient = new HttpClient();

        string bearerToken = await GoogleCloudHelper.GetBearerToken("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyBcWXnfmt7rN3JQ-b14brXypwg9Wmeu7Ow");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var response = httpClient.PostAsync(
            "https://us-central1-quiosquefood3000.cloudfunctions.net/delete-customer?cpf=" + customerDto.Cpf,
            null
        ).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao chamar função externa: {response.StatusCode}");
        }
    }
    public async Task<CustomerDto> UpdateCustomer(CustomerDto customerDto)
    {

        if (customerDto == null)
        {
            throw new ArgumentNullException("O cliente não pode ser nulo");
        }

        if (string.IsNullOrWhiteSpace(customerDto.Cpf))
        {
            throw new ArgumentNullException("O CPF do cliente deve ser informado");
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
        var payload = new
        {
            name = customerDto.Name,
            email = customerDto.Email,
            cpf = customerDto.Cpf
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8,
        "application/json");

        using var httpClient = new HttpClient();

        string bearerToken = await GoogleCloudHelper.GetBearerToken("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyBcWXnfmt7rN3JQ-b14brXypwg9Wmeu7Ow");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var response = httpClient.PostAsync(
            "https://us-central1-quiosquefood3000.cloudfunctions.net/update-customer?cpf=" + customerDto.Cpf,
            content
        ).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao chamar função externa: {response.StatusCode}");
        }
        return customerDto;
    }
}