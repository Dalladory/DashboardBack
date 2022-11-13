using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Data.Data.ViewModels
{
    public class TokenRequestVM
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
