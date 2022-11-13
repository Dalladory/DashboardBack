using Compass.Data.Data.ViewModels;
using FluentValidation;

namespace Compass.Data.Validation
{
    public class RegisterUserValidation : AbstractValidator<RegisterUserVM>
    {
        public RegisterUserValidation()
        {
            RuleFor(r => r.Email).EmailAddress().NotEmpty();
            RuleFor(r => r.Password).NotEmpty().MinimumLength(6);
            RuleFor(r => r.ConfirmPassword).NotEmpty().MinimumLength(6);
        }
    }
}
