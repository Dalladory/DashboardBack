using Compass.Data.Data.ViewModels;
using FluentValidation;

namespace Compass.Data.Validation
{
    public class ChangePasswordValidation : AbstractValidator<ChangePasswordVM>
    {
        public ChangePasswordValidation()
        {
            RuleFor(u => u.NewPassword).NotEmpty().MinimumLength(6);
            RuleFor(u => u.CurrentPassword).NotEmpty().MinimumLength(6);
            RuleFor(u => u.Id).NotEmpty();
        }
    }
}
