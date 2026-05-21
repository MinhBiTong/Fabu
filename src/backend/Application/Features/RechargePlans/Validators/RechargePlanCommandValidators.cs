using Application.Features.RechargePlans.Commands;
using FluentValidation;

namespace Application.Features.RechargePlans.Validators;

public sealed class CreateRechargePlanCommandValidator : AbstractValidator<CreateRechargePlanCommand>
{
    public CreateRechargePlanCommandValidator()
    {
        RuleFor(command => command.PlanName).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Amount).GreaterThan(0);
        RuleFor(command => command.BonusAmount).GreaterThanOrEqualTo(0);
        RuleFor(command => command.ValidityDays).GreaterThan(0).When(command => command.ValidityDays.HasValue);
    }
}

public sealed class UpdateRechargePlanCommandValidator : AbstractValidator<UpdateRechargePlanCommand>
{
    public UpdateRechargePlanCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.PlanName).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Amount).GreaterThan(0);
        RuleFor(command => command.BonusAmount).GreaterThanOrEqualTo(0);
        RuleFor(command => command.ValidityDays).GreaterThan(0).When(command => command.ValidityDays.HasValue);
    }
}

public sealed class DeleteRechargePlanCommandValidator : AbstractValidator<DeleteRechargePlanCommand>
{
    public DeleteRechargePlanCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
