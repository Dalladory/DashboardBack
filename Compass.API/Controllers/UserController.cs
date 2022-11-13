using Compass.Data.Data.ViewModels;
using Compass.Data.Validation;
using Compass.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Compass.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserVM model)
        {
            var validator = new RegisterUserValidation();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = validationResult.Errors[0].ErrorMessage
                });
            }
            
            var result = await _userService.RegisterUserAsync(model);
            if (result.IsSuccess) 
            {
                return Ok(result); 
            }
            
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserVM model)
        {
            var validator = new LoginUserValidation();
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = validationResult.Errors[0].ErrorMessage
                });
            }
            var result = await _userService.LoginUserAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody]  string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var result = await _userService.ForgotPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordVM model)
        {
            var validator = new ResetPasswordValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = validationResult.Errors[0].ErrorMessage
                });
            }
            var result = await _userService.ResetPasswordAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);  
            }
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRequestVM model)
        {
            var validator = new TokenRequestValidation();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = validationResult.Errors[0].ErrorMessage
                });
            }

            var result = await _userService.RefreshTokenAsync(model);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [Authorize(Roles = "Administrators")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsersAsync(int start, int end, bool isAll = false)
        {
            var result = await _userService.GetUsersAsync(start, end, isAll);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet("GetUsersQuantity")]
        public async Task<IActionResult> GetUsersQuantityAsync()
        {
            var result = await _userService.GetUsersQuantityAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfileAsync(string userId)
        {
            var result = await _userService.GetUserProfileAsync(userId);

            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileAsync(UserProfileVM model)
        {
            var validator = new UpdateUserValidation();
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = validationResult.Errors[0].ErrorMessage
                });
            }
            var result = await _userService.UpdateUserProfileAsync(model);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //[Authorize(Roles ="Administrators")]
        [Authorize(Roles = "Administrators")]
        [HttpGet("DeleteUserForAdmin")]
        public async Task<IActionResult> DeleteUserForAdminAsync(string userId)
        {
            var result = await _userService.DeleteUserForAdminAsync(userId);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost("UserEditForAdministrator")]
        public async Task<IActionResult> UserEditForAdministratorAsync([FromBody] UserProfileForAdminVM model)
        {
            var result = await _userService.UserEditForAdministratorAsync(model);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordVM model)
        {
            var validator = new ChangePasswordValidation();
            var validationResult = validator.Validate(model);
            
            if(!validationResult.IsValid)
            {
                return BadRequest(new ServiceResponse()
                {
                    IsSuccess = false,
                    Message = validationResult.Errors[0].ErrorMessage
                });
            }
            var result = await _userService.ChangePasswordAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


        //[AllowAnonymous]
        //[HttpGet("BlockUserForAdmin")]
        //public async Task<IActionResult> BlockUserForAdminAsync(string userId)
        //{
        //    var result = await _userService.BlockUserForAdminAsync(userId);
        //    if (result.IsSuccess)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result);
        //}


    }
}
