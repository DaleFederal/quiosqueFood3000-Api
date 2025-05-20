
using System.ComponentModel;

namespace QuiosqueFood3000.Domain.Entities.Enums
{
    public enum RemittanceStatus
    {
        [Description("Emitida")] Emitted,
        [Description("Paga")] Payed,
        [Description("Cancelada")] Cancelled,
        [Description("Erro de cobrança")] Error
    }
}
