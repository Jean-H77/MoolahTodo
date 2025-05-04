using FluentValidation;

namespace Core.Dto.Validators;

// https://docs.fluentvalidation.net/en/latest/aspnet.html#minimal-apis
public class TodoDtoValidator : AbstractValidator<TodoDto>
{
    public TodoDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
    }
}