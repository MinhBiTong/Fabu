using Application.DTOs.Requests.LoginRequest;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.LoginValidator
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
            RuleFor(x => x.Password).NotEmpty().NotEmpty().MinimumLength(6).MaximumLength(100);
        }
    }
}
