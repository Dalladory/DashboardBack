using Compass.Data.Data.ViewModels;
using FluentValidation;

namespace Compass.Data.Validation
{
    public class UpdateUserValidation : AbstractValidator<UserProfileVM>
    {
        public UpdateUserValidation()
        {
            //RuleFor(r => r.PhoneNumber).NotEmpty();
        }
    }
}
