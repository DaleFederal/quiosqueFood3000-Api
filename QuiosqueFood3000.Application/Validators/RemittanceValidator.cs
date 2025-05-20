using FluentValidation;
using QuiosqueFood3000.Domain.Entities;

namespace QuiosqueFood3000.Api.Validators;
public class RemittanceValidator : AbstractValidator<Remittance>
{
    public RemittanceValidator()
    {
        RuleFor(remittance => remittance.Value).GreaterThan(0).WithMessage("A remessa deve ter valor maior do que 0");
    }
}