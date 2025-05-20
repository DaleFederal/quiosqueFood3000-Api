using QuiosqueFood3000.Domain.Entities;

namespace QuiosqueFood3000.Infraestructure.Repositories.Interfaces;

public interface IRemittanceRepository
{
    Task<Remittance?> GetRemittancebyId(int Id);
    Task<Remittance?> GetRemittancebyOrderId(int orderId);
    Remittance RegisterRemittance(Remittance Remittance);
    void RemoveRemittance(Remittance Remittance);
    Remittance UpdateRemittance(Remittance Remittance);
}