using Compass.Data.Data.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Data.Validation
{
    public class TokenRequestValidation : AbstractValidator<TokenRequestVM>
    {
        public TokenRequestValidation()
        {
            RuleFor(r => r.Token).NotEmpty();
            RuleFor(r => r.RefreshToken).NotEmpty();
        }
    }
}
