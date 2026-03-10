using Application.DTOs.Requests.UserRequest;
using FluentValidation;

namespace Application.Validators.UserValidator
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
            RuleFor(x => x.PasswordHash).NotEmpty().EmailAddress().MaximumLength(200);
        }
    }
}
