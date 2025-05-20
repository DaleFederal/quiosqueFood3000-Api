using QuiosqueFood3000.Api.DTOs;
using QuiosqueFood3000.Api.Services.Interfaces;
using QuiosqueFood3000.Api.Validators;
using QuiosqueFood3000.Domain.Entities;
using QuiosqueFood3000.Domain.Entities.Enums;
using QuiosqueFood3000.Infraestructure.Repositories.Interfaces;

namespace QuiosqueFood3000.Api.Services;

public class RemittanceService(IRemittanceRepository remittanceRepository, IOrderService orderService) : IRemittanceService
{
    public async Task<RemittanceDto?> GetRemittanceById(int id)
    {

        var remittance = await remittanceRepository.GetRemittancebyId(id);

        return remittance == null
            ? null
            : new RemittanceDto()
            {
                Id = remittance.Id.ToString(),
                Order = remittance.Order ?? throw new ArgumentNullException(nameof(remittance.Order)),
                Value = remittance.Value,
                ExternalId = remittance.ExternalId ?? throw new ArgumentNullException(nameof(remittance.ExternalId)),
                QrCode = remittance.QrCode,
                RemittanceStatus = remittance.RemittanceStatus
            };
    }
    public async Task<RemittanceDto?> GetRemittanceByOrderId(int orderId)
    {

        var remittance = await remittanceRepository.GetRemittancebyOrderId(orderId);

        return remittance == null
            ? null
            : new RemittanceDto()
            {
                Id = remittance.Id.ToString(),
                Order = remittance.Order ?? throw new ArgumentNullException(nameof(remittance.Order)),
                Value = remittance.Value,
                ExternalId = remittance.ExternalId,
                QrCode = remittance.QrCode,
                RemittanceStatus = remittance.RemittanceStatus
            };
    }
    // public async Task<RemittanceDto> RegisterPayment(int id)
    // {
    //     var remittance = await remittanceRepository.GetRemittancebyId(id);
    //     if (remittance == null)
    //     {
    //         throw new InvalidOperationException("Remessa não foi encontrada");
    //     }
    //     if (remittance.RemittanceStatus != RemittanceStatus.Emitted)
    //     {
    //         throw new InvalidOperationException("Status da remessa não mpermite pagamento");
    //     }
    //
    //     remittance.RemittanceStatus = RemittanceStatus.Payed;
    //     remittance.PaymentDate = DateTime.UtcNow;
    //
    //     remittanceRepository.UpdateRemittance(remittance);
    //
    //     if (remittance.Order == null)
    //     {
    //         throw new InvalidOperationException("A remessa não está ligada à um pedido");
    //     }
    //     else
    //     {
    //         await orderService.RegisterPayment(remittance.Order.Id);
    //     }
    //
    //     return new RemittanceDto()
    //     {
    //         Id = remittance.Id.ToString(),
    //         RemittanceStatus = remittance.RemittanceStatus,
    //         Value = remittance.Value,
    //         ExternalId = remittance.ExternalId,
    //         QrCode = remittance.QrCode,
    //         Order = remittance.Order
    //     };
    // }
    public RemittanceDto RegisterRemittance(RemittanceDto remittanceDto)
    {

        RemittanceDtoValidator remittanceDtoValidator = new RemittanceDtoValidator();
        var resultRemittanceDto = remittanceDtoValidator.Validate(remittanceDto);

        if (!resultRemittanceDto.IsValid)
        {
            throw new InvalidDataException(resultRemittanceDto.ToString());
        }

        Remittance remittance = new Remittance()
        {
            Order = remittanceDto.Order,
            Value = remittanceDto.Value,
            GenerateDate = remittanceDto.GenerateDate ?? throw new ArgumentNullException(nameof(remittanceDto.GenerateDate)),
            ExternalId = remittanceDto.ExternalId,
            QrCode = remittanceDto.QrCode,
            RemittanceStatus = remittanceDto.RemittanceStatus
        };
        RemittanceValidator remittanceValidator = new RemittanceValidator();
        var resultRemittance = remittanceValidator.Validate(remittance);

        if (!resultRemittance.IsValid)
        {
            throw new InvalidDataException(resultRemittance.ToString());
        }
        remittance = remittanceRepository.RegisterRemittance(remittance);
        return new RemittanceDto()
        {
            Id = remittance.Id.ToString(),
            Order = remittance.Order ?? throw new ArgumentNullException(nameof(remittance.Order)),
            Value = remittance.Value,
            GenerateDate = remittance.GenerateDate,
            ExternalId = remittance.ExternalId,
            QrCode = remittance.QrCode,
            RemittanceStatus = remittance.RemittanceStatus
        };
    }
    public void RemoveRemittance(RemittanceDto remittanceDto)
    {
        if (string.IsNullOrEmpty(remittanceDto.Id))
        {
            throw new ArgumentNullException(nameof(remittanceDto.Id), "Id não pode ser nulo ou vazio");
        }

        Remittance remittance = new Remittance()
        {
            Id = int.Parse(remittanceDto.Id)
        };
        remittanceRepository.RemoveRemittance(remittance);
    }
    public RemittanceDto UpdateRemittance(RemittanceDto remittanceDto)
    {
        if (remittanceDto == null)
        {
            throw new ArgumentNullException("A remessa deve ser informada");
        }

        if (string.IsNullOrEmpty(remittanceDto.Id))
        {
            throw new ArgumentNullException(nameof(remittanceDto.Id), "Id não pode ser nulo ou vazio");
        }

        RemittanceDtoValidator remittanceDtoValidator = new RemittanceDtoValidator();
        var resultRemittanceDto = remittanceDtoValidator.Validate(remittanceDto);

        if (!resultRemittanceDto.IsValid)
        {
            throw new InvalidDataException(resultRemittanceDto.ToString());
        }

        Remittance remittance = new Remittance()
        {
            Id = int.Parse(remittanceDto.Id),
            Order = remittanceDto.Order,
            Value = remittanceDto.Value,
            ExternalId = remittanceDto.ExternalId,
            QrCode = remittanceDto.QrCode,
            RemittanceStatus = remittanceDto.RemittanceStatus
        };
        RemittanceValidator remittanceValidator = new RemittanceValidator();
        var resultRemittance = remittanceValidator.Validate(remittance);

        if (!resultRemittance.IsValid)
        {
            throw new InvalidDataException(resultRemittance.ToString());
        }
        remittance = remittanceRepository.UpdateRemittance(remittance);
        return new RemittanceDto()
        {
            Id = remittance.Id.ToString(),
            Order = remittance.Order ?? throw new ArgumentNullException(nameof(remittance.Order)),
            Value = remittance.Value,
            ExternalId = remittance.ExternalId,
            QrCode = remittance.QrCode,
            RemittanceStatus = remittance.RemittanceStatus
        };
    }
    public RemittanceDto Collect(OrderDto orderDto)
    {
        ArgumentNullException.ThrowIfNull(orderDto);
        ArgumentNullException.ThrowIfNull(orderDto.Id);

        Order order = new Order()
        {
            Id = int.Parse(orderDto.Id),
            TypeOfIdentification = orderDto.TypeOfIdentification ?? throw new ArgumentNullException(nameof(orderDto.TypeOfIdentification)),
            Customer = orderDto.Customer,
            AnonymousIdentification = orderDto.AnonymousIdentification,
            OrderSolicitation = orderDto.OrderSolicitation ?? throw new ArgumentNullException(nameof(orderDto.OrderSolicitation)),
            OrderStatus = orderDto.OrderStatus ?? throw new ArgumentNullException(nameof(orderDto.OrderStatus)),
            OrderItemsList = orderDto.OrderItemsList,
            TotalValue = orderDto.TotalValue ?? 0,
            InitialDate = orderDto.InitialDate ?? throw new ArgumentNullException(nameof(orderDto.InitialDate)),
            EndDate = orderDto.EndDate ?? null,
            PaymentStatus = orderDto.PaymentStatus ?? throw new ArgumentNullException(nameof(orderDto.PaymentStatus)),
        };

        //AQUI FICARIA CHAMADA INTEGRAÇÂO COM API DE INSTITUIÇÃO FINANCEIRA
        //RETORNO SERIA USADO PARA COMPOR A REMESSA
        RemittanceDto remittanceDto = new RemittanceDto()
        {
            RemittanceStatus = RemittanceStatus.Emitted,
            Value = orderDto.TotalValue ?? throw new ArgumentNullException(nameof(orderDto.TotalValue)),
            Order = order,
            ExternalId = Guid.NewGuid().ToString(),
            QrCode = Guid.NewGuid().ToString(),
            GenerateDate = DateTime.UtcNow,
        };
        remittanceDto = this.RegisterRemittance(remittanceDto);
        return remittanceDto;
    }
}
