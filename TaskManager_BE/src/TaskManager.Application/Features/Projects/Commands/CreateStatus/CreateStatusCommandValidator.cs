using FluentValidation;

namespace TaskManager.Application.Features.Projects.Commands.CreateStatus;

public sealed class CreateStatusCommandValidator : AbstractValidator<CreateStatusCommand>
{
    public CreateStatusCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Color).NotEmpty().Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color (e.g. #3B82F6).");
    }
}
