using FluentValidation;
using QuiosqueFood3000.Api.DTOs;

namespace QuiosqueFood3000.Api.Validators;
public class RemittanceDtoValidator : AbstractValidator<RemittanceDto>
{
    public RemittanceDtoValidator()
    {
        RuleFor(remittanceDto => remittanceDto.Value).GreaterThan(0).WithMessage("A remessa deve ter valor maior do que 0");
    }
}