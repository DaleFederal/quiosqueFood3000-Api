using QuiosqueFood3000.Api.DTOs;

namespace QuiosqueFood3000.Api.Services.Interfaces;

public interface IRemittanceService
{
    Task<RemittanceDto?> GetRemittanceById(int id);
    Task<RemittanceDto?> GetRemittanceByOrderId(int orderId);
    RemittanceDto RegisterRemittance(RemittanceDto remittanceDto);
    void RemoveRemittance(RemittanceDto remittanceDto);
    RemittanceDto UpdateRemittance(RemittanceDto remittanceDto);
    // Task<RemittanceDto> RegisterPayment(int id);
    RemittanceDto Collect(OrderDto orderDto);
}