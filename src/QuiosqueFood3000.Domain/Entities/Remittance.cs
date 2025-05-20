using QuiosqueFood3000.Domain.Entities.Enums;

namespace QuiosqueFood3000.Domain.Entities;

public class Remittance
{
    public int Id { get; set; }
    public RemittanceStatus RemittanceStatus { get; set; }
    public decimal Value { get; set; }
    public string? ExternalId { get; set; }
    public string? QrCode { get; set; }
    public DateTime GenerateDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public Order? Order { get; set; }
}